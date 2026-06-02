using Bengos.Models;
using BillingAndPayment.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BillingAndPayment
{
    public partial class BillingWindow : Window
    {
        private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

        public ObservableCollection<OrderItem> OrderItems { get; } = new();
        public ObservableCollection<Dish> Dishes { get; } = new();

        private double discountPercent;
        private bool isEditing;
        private int? tableId;
        private int? currentOrderId;

        public double CurrentDiscount => discountPercent;

        // Constructor implicit — fara masa
        public BillingWindow() : this(null) { }

        // Constructor cu masa (masa noua, fara comanda existenta)
        public BillingWindow(int? tableId)
        {
            InitializeComponent();
            this.tableId = tableId;
            if (tableId.HasValue)
                Title = $"Billing — Table {tableId.Value}";

            LoadDishes();
            CmbDish.ItemsSource = Dishes;
            CmbDish.SelectedIndex = 0;
            DgOrder.ItemsSource = OrderItems;

            BtnEditQty.IsEnabled = false;
            BtnDeleteItem.IsEnabled = false;

            OrderItems.CollectionChanged += (s, e) =>
            {
                BtnEditQty.IsEnabled = OrderItems.Count > 0;
                BtnDeleteItem.IsEnabled = OrderItems.Count > 0 && DgOrder.SelectedItem != null;
            };
            DgOrder.SelectionChanged += DgOrder_SelectionChanged;
        }

        // Constructor cu masa + comanda existenta (deschis din TableWindow)
        public BillingWindow(int? tableId, int orderId, double discount)
        {
            InitializeComponent();
            this.tableId = tableId;
            this.currentOrderId = orderId;
            this.discountPercent = discount;

            if (tableId.HasValue)
                Title = $"Billing — Table {tableId.Value}";

            LoadDishes();
            CmbDish.ItemsSource = Dishes;
            CmbDish.SelectedIndex = 0;
            DgOrder.ItemsSource = OrderItems;

            LoadOrderItemsFromDb();
            RefreshTotals();

            BtnEditQty.IsEnabled = OrderItems.Count > 0;
            BtnDeleteItem.IsEnabled = false;

            OrderItems.CollectionChanged += (s, e) =>
            {
                BtnEditQty.IsEnabled = OrderItems.Count > 0;
                BtnDeleteItem.IsEnabled = OrderItems.Count > 0 && DgOrder.SelectedItem != null;
            };
            DgOrder.SelectionChanged += DgOrder_SelectionChanged;
        }

        // ──────────────────────────────────────────────
        // LOAD
        // ──────────────────────────────────────────────

        private void LoadDishes()
        {
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "SELECT Id, Name, Price, Category FROM Dishes ORDER BY Category, Name", conn);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Dishes.Add(new Dish
                    {
                        Name = rdr["Name"]?.ToString() ?? "",
                        Price = Convert.ToDouble(rdr["Price"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dishes: " + ex.Message);
            }
        }

        private void LoadOrderItemsFromDb()
        {
            if (!currentOrderId.HasValue) return;
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "SELECT Name, Quantity, Price FROM OrderItems WHERE OrderId=@oid", conn);
                cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OrderItems.Add(new OrderItem
                    {
                        Name = rdr["Name"]?.ToString() ?? "",
                        Quantity = Convert.ToInt32(rdr["Quantity"]),
                        Price = Convert.ToDouble(rdr["Price"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order items: " + ex.Message);
            }
        }

        // ──────────────────────────────────────────────
        // EVENTS
        // ──────────────────────────────────────────────

        private void DgOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnDeleteItem.IsEnabled = DgOrder.SelectedItem != null;
            if (isEditing && DgOrder.SelectedItem != null)
            {
                DgOrder.BeginEdit();
                var cell = DgOrder.Columns[1].GetCellContent(DgOrder.SelectedItem);
                cell?.Focus();
            }
        }

        private void CmbDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbDish.SelectedItem is Dish dish)
                TxtUnitPrice.Text = $"Unit Price: {dish.Price:0.00} RON";
        }

        private void BtnAddDish_Click(object sender, RoutedEventArgs e)
        {
            if (CmbDish.SelectedItem is not Dish dish)
            {
                MessageBox.Show("Please select a dish.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtQty.Text, out int qty) || qty <= 0 || qty > 100)
            {
                MessageBox.Show("Please enter a valid quantity (1-100).", "Invalid Quantity",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = OrderItems.FirstOrDefault(o => o.Name == dish.Name);
            if (existing != null)
            {
                if (existing.Quantity + qty > 100)
                {
                    MessageBox.Show("Total quantity for this dish cannot exceed 100.", "Quantity Limit",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                existing.Quantity += qty;
            }
            else
                OrderItems.Add(new OrderItem { Name = dish.Name, Quantity = qty, Price = dish.Price });

            DgOrder.Items.Refresh();
            RefreshTotals();

            if (currentOrderId.HasValue)
                SyncItemToDb(dish.Name, existing ?? OrderItems.Last());
        }

        private void SyncItemToDb(string name, OrderItem item)
        {
            if (!currentOrderId.HasValue) return;
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();
                using var check = new SqlCommand(
                    "SELECT Id FROM OrderItems WHERE OrderId=@oid AND Name=@name", conn);
                check.Parameters.AddWithValue("@oid", currentOrderId.Value);
                check.Parameters.AddWithValue("@name", name);
                var exists = check.ExecuteScalar();

                if (exists != null)
                {
                    using var upd = new SqlCommand(
                        "UPDATE OrderItems SET Quantity=@qty, Price=@price WHERE Id=@id", conn);
                    upd.Parameters.AddWithValue("@qty", item.Quantity);
                    upd.Parameters.AddWithValue("@price", item.Price);
                    upd.Parameters.AddWithValue("@id", (int)exists);
                    upd.ExecuteNonQuery();
                }
                else
                {
                    using var ins = new SqlCommand(
                        "INSERT INTO OrderItems (OrderId, Name, Quantity, Price) VALUES (@oid, @name, @qty, @price)", conn);
                    ins.Parameters.AddWithValue("@oid", currentOrderId.Value);
                    ins.Parameters.AddWithValue("@name", name);
                    ins.Parameters.AddWithValue("@qty", item.Quantity);
                    ins.Parameters.AddWithValue("@price", item.Price);
                    ins.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private void DgOrder_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.Column.Header.ToString() == "Qty" && e.EditingElement is TextBox cell)
                {
                    if (!int.TryParse(cell.Text, out int qty) || qty <= 0 || qty > 100)
                    {
                        e.Cancel = true;
                        cell.Text = (e.Row.Item as OrderItem)?.Quantity.ToString();
                    }
                    DgOrder.Items.Refresh();
                    RefreshTotals();
                }
            }
            catch { }
        }

        private void BtnEditQty_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing)
            {
                DgOrder.CommitEdit(DataGridEditingUnit.Row, true);
                isEditing = false;
                DgOrder.IsReadOnly = true;
                DgOrder.Background = System.Windows.Media.Brushes.Transparent;
                BtnEditQty.Content = "Edit Order";
                TxtUnitPrice.Text = "";
            }
            else
            {
                isEditing = true;
                TxtUnitPrice.Text = "Select an item to edit";
                TxtUnitPrice.Foreground = System.Windows.Media.Brushes.Gray;
                DgOrder.IsReadOnly = false;
                DgOrder.Background = System.Windows.Media.Brushes.LightYellow;
                BtnEditQty.Content = "Done Editing";
            }

            BtnAddDish.IsEnabled = !isEditing;
            BtnDiscount.IsEnabled = !isEditing;
            BtnPay.IsEnabled = !isEditing;
            BtnDeleteItem.IsEnabled = !isEditing;
        }

        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (DgOrder.SelectedItem is OrderItem item)
            {
                OrderItems.Remove(item);
                DgOrder.Items.Refresh();
                RefreshTotals();

                if (currentOrderId.HasValue)
                {
                    try
                    {
                        using var conn = new SqlConnection(connString);
                        conn.Open();
                        using var cmd = new SqlCommand(
                            "DELETE FROM OrderItems WHERE OrderId=@oid AND Name=@name", conn);
                        cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
                        cmd.Parameters.AddWithValue("@name", item.Name);
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                }
            }
        }

        private void BtnDiscount_Click(object sender, RoutedEventArgs e)
        {
            var popup = new Window
            {
                Title = "Add Discount",
                Width = 320,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var stack = new StackPanel { Margin = new Thickness(10) };
            stack.Children.Add(new TextBlock
            {
                Text = "Enter discount percentage:",
                Margin = new Thickness(0, 0, 0, 5)
            });

            var txtDisc = new TextBox
            {
                Text = discountPercent > 0 ? discountPercent.ToString() : "",
                Margin = new Thickness(0, 0, 0, 10)
            };
            stack.Children.Add(txtDisc);

            var lblCalc = new TextBlock
            {
                Text = "Calculated discount: 0.00",
                Foreground = System.Windows.Media.Brushes.DarkGreen
            };
            stack.Children.Add(lblCalc);

            txtDisc.TextChanged += (s, ev) =>
            {
                if (double.TryParse(txtDisc.Text, out double pct))
                {
                    double sub = OrderItems.Sum(i => i.Total);
                    lblCalc.Text = $"Calculated discount: {sub * pct / 100.0:0.00}";
                }
                else lblCalc.Text = "Calculated discount: 0.00";
            };

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };
            var btnApply = new Button { Content = "Apply", Width = 80, Height = 30, Margin = new Thickness(5), IsDefault = true };
            var btnCancel = new Button { Content = "Cancel", Width = 80, Height = 30, Margin = new Thickness(5), IsCancel = true };
            btnPanel.Children.Add(btnApply);
            btnPanel.Children.Add(btnCancel);
            stack.Children.Add(btnPanel);

            btnApply.Click += (s, ev) =>
            {
                if (double.TryParse(txtDisc.Text, out double pct) && pct >= 0 && pct <= 100)
                {
                    discountPercent = pct;
                    RefreshTotals();
                    MessageBox.Show($"Discount of {pct}% applied!", "Done",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    popup.Close();
                }
                else MessageBox.Show("Please enter a value between 0 and 100.", "Invalid",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            };

            popup.Content = stack;
            popup.ShowDialog();
        }

        private void BtnPay_Click(object sender, RoutedEventArgs e)
        {
            if (OrderItems.Count == 0)
            {
                MessageBox.Show("No items in the order.", "Empty Order",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double subtotalVal = OrderItems.Sum(i => i.Total);
            double totalVal = subtotalVal - (subtotalVal * discountPercent / 100.0);

            // Pasul 1: Dacă este o masă nouă și nu avem încă un Id de comandă în baza de date, 
            // creăm comanda ca fiind 'Pending' ca să avem un ID valid de trimis către PaymentWindow
            if (!currentOrderId.HasValue)
            {
                try
                {
                    using var conn = new SqlConnection(connString);
                    conn.Open();
                    using var cmd = new SqlCommand(
                        @"INSERT INTO Orders (Total, DiscountPercent, Status, TableId)
                          OUTPUT INSERTED.Id VALUES (@total, @disc, 'Pending', @tableId)", conn);
                    cmd.Parameters.AddWithValue("@total", totalVal);
                    cmd.Parameters.AddWithValue("@disc", discountPercent);
                    cmd.Parameters.AddWithValue("@tableId", (object?)tableId ?? DBNull.Value);
                    currentOrderId = (int)cmd.ExecuteScalar();

                    // Salvăm și itemele pentru această comandă nouă
                    foreach (var item in OrderItems)
                    {
                        using var itemCmd = new SqlCommand(
                            "INSERT INTO OrderItems (OrderId, Name, Quantity, Price) VALUES (@oid, @name, @qty, @price)", conn);
                        itemCmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
                        itemCmd.Parameters.AddWithValue("@name", item.Name);
                        itemCmd.Parameters.AddWithValue("@qty", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@price", item.Price);
                        itemCmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error initializing order for payment: " + ex.Message);
                    return;
                }
            }

            // Pasul 2: Deschidem fereastra de plată trimițând TableId și CurrentOrderId
            var payWindow = new PaymentWindow(
                totalVal,
                discountPercent,
                tableId ?? 0,
                currentOrderId.Value
            )
            { Owner = this };

            if (payWindow.ShowDialog() == true)
            {
                // Pasul 3: Curățăm interfața din BillingWindow deoarece plata și statusul mesei au fost deja rezolvate cu succes în PaymentWindow
                OrderItems.Clear();
                discountPercent = 0;
                currentOrderId = null;
                RefreshTotals();
                DialogResult = true;
                Close();
            }
        }

        private void SaveOrder(double total)
        {
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();

                if (currentOrderId.HasValue)
                {
                    using var cmd = new SqlCommand(
                        "UPDATE Orders SET Total=@total, DiscountPercent=@disc, Status='Paid' WHERE Id=@oid", conn);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@disc", discountPercent);
                    cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    using var cmd = new SqlCommand(
                        @"INSERT INTO Orders (Total, DiscountPercent, Status, TableId)
                          OUTPUT INSERTED.Id VALUES (@total, @disc, 'Paid', @tableId)", conn);
                    cmd.Parameters.AddWithValue("@total", total);
                    cmd.Parameters.AddWithValue("@disc", discountPercent);
                    cmd.Parameters.AddWithValue("@tableId", (object?)tableId ?? DBNull.Value);
                    int orderId = (int)cmd.ExecuteScalar();

                    foreach (var item in OrderItems)
                    {
                        using var itemCmd = new SqlCommand(
                            "INSERT INTO OrderItems (OrderId, Name, Quantity, Price) VALUES (@oid, @name, @qty, @price)", conn);
                        itemCmd.Parameters.AddWithValue("@oid", orderId);
                        itemCmd.Parameters.AddWithValue("@name", item.Name);
                        itemCmd.Parameters.AddWithValue("@qty", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@price", item.Price);
                        itemCmd.ExecuteNonQuery();
                    }
                }

                if (tableId.HasValue)
                {
                    using var updCmd = new SqlCommand(
                        "UPDATE Tables SET Status='Free' WHERE Id=@id", conn);
                    updCmd.Parameters.AddWithValue("@id", tableId.Value);
                    updCmd.ExecuteNonQuery();
                }

                OrderItems.Clear();
                discountPercent = 0;
                RefreshTotals();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving order: " + ex.Message, "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void RefreshTotals()
        {
            double subtotalVal = OrderItems.Sum(i => i.Total);
            double discountAmount = subtotalVal * discountPercent / 100.0;
            double totalVal = subtotalVal - discountAmount;

            TxtSubtotal.Text = $"{subtotalVal:0.00} RON";
            TxtDiscount.Text = discountPercent > 0
                ? $"-{discountAmount:0.00} ({discountPercent}%)"
                : "0.00";
            TxtTotal.Text = $"{totalVal:0.00} RON";
        }

        // ──────────────────────────────────────────────
        // INNER CLASSES
        // ──────────────────────────────────────────────
    }
}