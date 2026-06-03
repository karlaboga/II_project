using Bengos.Models;
using BillingAndPayment.Models;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BillingAndPayment;

public partial class TableWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;";
    private List<Table> tables = new();
    private List<Dish> allDishes = new();
    private int? selectedTableId;
    private int? currentOrderId;
    private int? currentOrderNumber;
    private double discountPercent;

    public static TableWindow Instance { get; private set; }

    public TableWindow()
    {
        InitializeComponent();
        Instance = this;
        LoadTables();
        LoadAllDishes();
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        LoadTables();
    }

    public void LoadTables()
    {
        tables.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT t.Id, t.TableNumber, t.Capacity, t.Status,
                       ISNULL(o.Status, '') AS OrderStatus
                FROM Tables t
                LEFT JOIN Orders o ON o.TableId = t.Id AND o.Status <> 'Paid'
                    AND o.Id = (SELECT MAX(Id) FROM Orders WHERE TableId = t.Id AND Status <> 'Paid')
                ORDER BY t.TableNumber", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                tables.Add(new Table
                {
                    Id = Convert.ToInt32(rdr["Id"]),
                    TableNumber = Convert.ToInt32(rdr["TableNumber"]),
                    Capacity = Convert.ToInt32(rdr["Capacity"]),
                    Status = rdr["Status"]?.ToString() ?? "Free",
                    OrderStatus = rdr["OrderStatus"]?.ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading tables: " + ex.Message);
        }

        TableList.ItemsSource = null;
        TableList.ItemsSource = tables;
        if (selectedTableId.HasValue)
        {
            var matchedTable = tables.FirstOrDefault(t => t.Id == selectedTableId.Value);
            if (matchedTable != null)
                matchedTable.IsSelected = true;
        }
        if (selectedTableId.HasValue)
        {
            var currentTable = tables.FirstOrDefault(t => t.Id == selectedTableId.Value);
            if (currentTable == null || currentTable.Status == "Free")
            {
                ResetOrderPanel();
            }
            else
            {
                try
                {
                    using var conn = new SqlConnection(connString);
                    conn.Open();
                    using var cmd = new SqlCommand(
                        "SELECT Id, OrderNumber, DiscountPercent, Status FROM Orders " +
                        "WHERE TableId=@tid AND Status <> 'Paid' ORDER BY Id DESC", conn);
                    cmd.Parameters.AddWithValue("@tid", selectedTableId.Value);
                    using var rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        currentOrderId = Convert.ToInt32(rdr["Id"]);
                        currentOrderNumber = Convert.ToInt32(rdr["OrderNumber"]);
                        discountPercent = Convert.ToDouble(rdr["DiscountPercent"]);
                        var orderStatus = rdr["Status"]?.ToString() ?? "";
                        TxtTableHeader.Text = orderStatus == "ReadyToServe"
                            ? $"Table {currentTable.TableNumber} — Order (Ready to Serve ✅)"
                            : $"Table {currentTable.TableNumber} — Order";
                    }
                    else
                    {
                        ResetOrderPanel();
                        return;
                    }
                }
                catch { }

                if (currentOrderId.HasValue)
                    LoadOrderItems();
            }
        }
    }

    private void LoadAllDishes()
    {
        allDishes.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Name, Price FROM Dishes ORDER BY Name", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                allDishes.Add(new Dish
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

    private void TableCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Border border && border.Tag is int tableId)
        {
            border.Focus();
            var table = tables.FirstOrDefault(t => t.Id == tableId);
            if (table == null) return;
            selectedTableId = tableId;

            if (table.Status == "Occupied")
            {
                OpenExistingOrder(tableId);
                return;
            }

            currentOrderId = null;
            currentOrderNumber = null;
            discountPercent = 0;
            DgOrder.ItemsSource = null;
            RefreshTotals();
            EnableOrderPanel(table);
        }
    }

    private void EnsureOrderExists(int tableId)
    {
        if (currentOrderId.HasValue) return;

        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            using var cmd = new SqlCommand(@"
                INSERT INTO Orders (Total, DiscountPercent, Status, TableId, OrderNumber)
                OUTPUT INSERTED.Id, INSERTED.OrderNumber
                VALUES (0, 0, 'Pending', @tid,
                    ISNULL((SELECT MAX(OrderNumber) FROM Orders WHERE CAST(ISNULL(OrderDate, GETDATE()) AS DATE) = CAST(GETDATE() AS DATE)), 0) + 1)", conn);
            cmd.Parameters.AddWithValue("@tid", tableId);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                currentOrderId = Convert.ToInt32(rdr["Id"]);
                currentOrderNumber = Convert.ToInt32(rdr["OrderNumber"]);
            }

            using var upd = new SqlCommand("UPDATE Tables SET Status='Occupied' WHERE Id=@id", conn);
            upd.Parameters.AddWithValue("@id", tableId);
            upd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error creating order on demand: " + ex.Message);
        }
    }

    private void OpenExistingOrder(int tableId)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT Id, DiscountPercent, OrderNumber FROM Orders WHERE TableId=@tid AND Status <> 'Paid' ORDER BY Id DESC", conn);
            cmd.Parameters.AddWithValue("@tid", tableId);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                currentOrderId = Convert.ToInt32(rdr["Id"]);
                discountPercent = Convert.ToDouble(rdr["DiscountPercent"]);
                currentOrderNumber = Convert.ToInt32(rdr["OrderNumber"]);
            }
            else
            {
                rdr.Close();
                currentOrderId = null;
                currentOrderNumber = null;
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading order: " + ex.Message);
        }

        var table = tables.FirstOrDefault(t => t.Id == tableId);
        if (table != null) EnableOrderPanel(table);
        LoadOrderItems();
        RefreshTotals();
    }

    private void EnableOrderPanel(Table table)
    {
        TxtTableHeader.Text = $"Table {table.TableNumber} — Order";
        OrderPanel.IsEnabled = true;
        OrderPanel.Opacity = 1;
        TxtSearch.Focus();
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filter = TxtSearch.Text?.Trim().ToLower() ?? "";
        if (string.IsNullOrEmpty(filter))
        {
            LstSearchResults.ItemsSource = null;
            return;
        }
        var results = allDishes
            .Where(d => d.Name.ToLower().Contains(filter))
            .Take(20)
            .ToList();
        LstSearchResults.ItemsSource = results;
    }

    private void BtnAddDish_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Dish dish && selectedTableId.HasValue)
        {
            try
            {
                EnsureOrderExists(selectedTableId.Value);

                using var conn = new SqlConnection(connString);
                conn.Open();
                using var check = new SqlCommand(
                    "SELECT Id, Quantity, ISNULL(StatusItem, 'Pending') AS StatusItem FROM OrderItems WHERE OrderId=@oid AND Name=@name", conn);
                check.Parameters.AddWithValue("@oid", currentOrderId.Value);
                check.Parameters.AddWithValue("@name", dish.Name);
                using var rdr = check.ExecuteReader();
                if (rdr.Read())
                {
                    string existingStatus = rdr["StatusItem"]?.ToString() ?? "Pending";
                    if (existingStatus == "Ready")
                    {
                        rdr.Close();
                        using var ins = new SqlCommand(
                            "INSERT INTO OrderItems (OrderId, Name, Quantity, Price, StatusItem) VALUES (@oid, @name, 1, @price, 'Pending')", conn);
                        ins.Parameters.AddWithValue("@oid", currentOrderId.Value);
                        ins.Parameters.AddWithValue("@name", dish.Name);
                        ins.Parameters.AddWithValue("@price", dish.Price);
                        ins.ExecuteNonQuery();
                    }
                    else
                    {
                        int itemId = Convert.ToInt32(rdr["Id"]);
                        int qty = Convert.ToInt32(rdr["Quantity"]) + 1;
                        rdr.Close();
                        using var upd = new SqlCommand(
                            "UPDATE OrderItems SET Quantity=@qty WHERE Id=@id", conn);
                        upd.Parameters.AddWithValue("@qty", qty);
                        upd.Parameters.AddWithValue("@id", itemId);
                        upd.ExecuteNonQuery();
                    }
                }
                else
                {
                    rdr.Close();
                    using var ins = new SqlCommand(
                        "INSERT INTO OrderItems (OrderId, Name, Quantity, Price, StatusItem) VALUES (@oid, @name, 1, @price, 'Pending')", conn);
                    ins.Parameters.AddWithValue("@oid", currentOrderId.Value);
                    ins.Parameters.AddWithValue("@name", dish.Name);
                    ins.Parameters.AddWithValue("@price", dish.Price);
                    ins.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding dish: " + ex.Message);
            }

            LoadTables();
        }
    }

    private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is OrderItem item && currentOrderId.HasValue)
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
            catch (Exception ex)
            {
                MessageBox.Show("Error removing item: " + ex.Message);
            }
            LoadOrderItems();
        }
    }

    public void LoadOrderItems()
    {
        var items = new System.Collections.ObjectModel.ObservableCollection<OrderItem>();
        if (!currentOrderId.HasValue) { DgOrder.ItemsSource = items; return; }
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT Name, Quantity, Price, ISNULL(StatusItem, 'Pending') AS StatusItem FROM OrderItems WHERE OrderId=@oid ORDER BY Id", conn);
            cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                items.Add(new OrderItem
                {
                    Name = rdr["Name"]?.ToString() ?? "",
                    Quantity = Convert.ToInt32(rdr["Quantity"]),
                    Price = Convert.ToDouble(rdr["Price"]),
                    StatusItem = rdr["StatusItem"]?.ToString() ?? "Pending"
                });
            }
            rdr.Close();

            var insufficientDishes = new HashSet<string>();
            using var checkCmd = new SqlCommand(@"
                SELECT oi.Name AS DishName
                FROM OrderItems oi
                INNER JOIN Dishes d ON oi.Name = d.Name
                INNER JOIN DishIngredients di ON di.DishId = d.Id
                INNER JOIN Produses p ON p.Id = di.ProductId
                WHERE oi.OrderId = @oid
                  AND p.Quantity < (di.QuantityRequired * oi.Quantity)
                GROUP BY oi.Name", conn);
            checkCmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
            using var checkReader = checkCmd.ExecuteReader();
            while (checkReader.Read())
            {
                string dishName = checkReader["DishName"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(dishName))
                    insufficientDishes.Add(dishName);
            }
            checkReader.Close();

            bool hasInsufficient = false;
            foreach (var item in items)
            {
                if (insufficientDishes.Contains(item.Name))
                {
                    item.HasInsufficientIngredients = true;
                    hasInsufficient = true;
                }
            }
            bool allReady = items.Count > 0 && items.All(i => i.StatusItem == "Ready");
            BtnToKitchen.IsEnabled = !hasInsufficient && !allReady;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading items: " + ex.Message);
        }
        DgOrder.ItemsSource = items;
        RefreshTotals();
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
        stack.Children.Add(new TextBlock { Text = "Enter discount percentage:", Margin = new Thickness(0, 0, 0, 5) });
        var txtDisc = new TextBox { Text = discountPercent > 0 ? discountPercent.ToString() : "", Margin = new Thickness(0, 0, 0, 10) };
        stack.Children.Add(txtDisc);
        var lblCalc = new TextBlock { Text = "Calculated discount: 0.00", Foreground = System.Windows.Media.Brushes.DarkGreen };
        stack.Children.Add(lblCalc);
        txtDisc.TextChanged += (s, ev) =>
        {
            if (double.TryParse(txtDisc.Text, out double pct))
            {
                double sub = GetSubtotal();
                lblCalc.Text = $"Calculated discount: {sub * pct / 100.0:0.00}";
            }
            else lblCalc.Text = "Calculated discount: 0.00";
        };
        var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 10, 0, 0) };
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
                MessageBox.Show($"Discount of {pct}% applied!", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                popup.Close();
            }
            else MessageBox.Show("Please enter a value between 0 and 100.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
        };
        popup.Content = stack;
        popup.ShowDialog();
    }

    private void BtnBilling_Click(object sender, RoutedEventArgs e)
    {
        if (!currentOrderId.HasValue || !selectedTableId.HasValue)
        {
            MessageBox.Show("Vă rugăm să adăugați produse în comandă înainte de facturare!", "Panou Gol", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var billing = new BillingWindow(selectedTableId.Value, currentOrderId.Value, discountPercent);
        billing.Owner = this;
        billing.ShowDialog();
        discountPercent = billing.CurrentDiscount;
        LoadOrderItems();
        RefreshTotals();
    }

    private void FinalizeOrder(double total)
    {
        if (!currentOrderId.HasValue || !selectedTableId.HasValue) return;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE Orders SET Total=@total, DiscountPercent=@disc, Status='Paid' WHERE Id=@oid", conn);
            cmd.Parameters.AddWithValue("@total", total);
            cmd.Parameters.AddWithValue("@disc", discountPercent);
            cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
            cmd.ExecuteNonQuery();

            using var upd = new SqlCommand(
                "UPDATE Tables SET Status='Free' WHERE Id=@id", conn);
            upd.Parameters.AddWithValue("@id", selectedTableId.Value);
            upd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error finalizing order: " + ex.Message);
        }

        ResetOrderPanel();
        LoadTables();
    }

    private void ResetOrderPanel()
    {
        currentOrderId = null;
        currentOrderNumber = null;
        selectedTableId = null;
        discountPercent = 0;
        OrderPanel.IsEnabled = false;
        OrderPanel.Opacity = 0.4;
        TxtTableHeader.Text = "";
        TxtSearch.Text = "";
        LstSearchResults.ItemsSource = null;
        DgOrder.ItemsSource = null;
    }

    private double GetSubtotal()
    {
        var items = DgOrder.ItemsSource as System.Collections.ObjectModel.ObservableCollection<OrderItem>;
        return items?.Sum(i => i.Total) ?? 0;
    }

    private void RefreshTotals()
    {
        double subtotalVal = GetSubtotal();
        double discountAmount = subtotalVal * discountPercent / 100.0;
        double totalVal = subtotalVal - discountAmount;
        TxtSubtotal.Text = $"{subtotalVal:0.00} RON";
        TxtDiscount.Text = discountPercent > 0 ? $"-{discountAmount:0.00} ({discountPercent}%)" : "0.00 RON";
        TxtTotal.Text = $"{totalVal:0.00} RON";
    }

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        if (currentOrderId.HasValue && selectedTableId.HasValue)
        {
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();
                using var cmd = new SqlCommand(
                    "DELETE FROM Orders WHERE Id=@oid AND Status='Pending' " +
                    "AND NOT EXISTS (SELECT 1 FROM OrderItems WHERE OrderId=@oid)", conn);
                cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
                cmd.ExecuteNonQuery();

                using var upd = new SqlCommand(
                    "UPDATE Tables SET Status='Free' WHERE Id=@id AND Status='Occupied' " +
                    "AND NOT EXISTS (SELECT 1 FROM Orders WHERE TableId=@id AND (Status='Pending' OR Status='ToKitchen'))", conn);
                upd.Parameters.AddWithValue("@id", selectedTableId.Value);
                upd.ExecuteNonQuery();
            }
            catch { }
        }
        Close();
    }

    private void BtnToKitchen_Click(object sender, RoutedEventArgs e)
    {
        if (!currentOrderId.HasValue)
        {
            MessageBox.Show("Nu există o comandă activă pentru a fi trimisă la bucătărie!", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var items = DgOrder.ItemsSource as System.Collections.ObjectModel.ObservableCollection<OrderItem>;
        if (items == null || items.Count == 0)
        {
            MessageBox.Show("Comanda este goală! Adăugați produse înainte de a o trimite la bucătărie.", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            using var cmd = new SqlCommand(
                "UPDATE Orders SET Status = 'ToKitchen', OrderDate = GETDATE() WHERE Id = @oid", conn);
            cmd.Parameters.AddWithValue("@oid", currentOrderId.Value);
            cmd.ExecuteNonQuery();

            MessageBox.Show($"Comanda #{currentOrderNumber} a fost trimisă cu succes la bucătărie!", "Trimis", MessageBoxButton.OK, MessageBoxImage.Information);

            LoadTables();
            LoadOrderItems();
            RefreshTotals();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Eroare la trimiterea comenzii către bucătărie: " + ex.Message, "Eroare SQL", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}