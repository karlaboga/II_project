using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using BillingAndPayment.Models;
namespace BillingAndPayment;
public partial class BillingWindow : Window
{
    public ObservableCollection<OrderItem> OrderItems { get; } = new();
    private double discountPercent;
    private bool isEditing;
    public List<Dish> Dishes { get; } = new()
    {
        new Dish { Name = "Classic Burger",          Price = 12.99 },
        new Dish { Name = "Chicken Pasta",           Price = 14.50 },
        new Dish { Name = "Caesar Salad",            Price = 9.99  },
        new Dish { Name = "Beef Burger",             Price = 15.99 },
        new Dish { Name = "Cappuccino",              Price = 4.50  },
        new Dish { Name = "Chocolate Sundae",        Price = 6.99  },
        new Dish { Name = "Grilled Chicken Salad",   Price = 11.50 },
        new Dish { Name = "Iced Coffee",             Price = 5.50  },
    };
    public BillingWindow()
    {
        InitializeComponent();
        CmbDish.ItemsSource = Dishes;
        CmbDish.SelectedIndex = 0;
        DgOrder.ItemsSource = OrderItems;
    }
    private void CmbDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbDish.SelectedItem is Dish dish)
            TxtUnitPrice.Text = $"Unit Price: ${dish.Price:0.00}";
    }
    private void BtnAddDish_Click(object sender, RoutedEventArgs e)
    {
        if (CmbDish.SelectedItem is not Dish dish)
        {
            MessageBox.Show("Please select a dish.", "No Selection",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(TxtQty.Text, out int qty) || qty <= 0)
        {
            MessageBox.Show("Please enter a valid quantity.", "Invalid Quantity",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var existing = OrderItems.FirstOrDefault(o => o.Name == dish.Name);
        if (existing != null)
            existing.Quantity += qty;
        else
            OrderItems.Add(new OrderItem { Name = dish.Name, Quantity = qty, Price = dish.Price });
        DgOrder.Items.Refresh();
        RefreshTotals();
    }
    private void DgOrder_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.Column.Header.ToString() == "Qty" && e.EditingElement is TextBox cell)
        {
            if (int.TryParse(cell.Text, out int qty) && qty > 0)
            {
                if (e.Row.Item is OrderItem item)
                    item.Quantity = qty;
            }
            DgOrder.Items.Refresh();
            RefreshTotals();
        }
    }
    private void BtnEditQty_Click(object sender, RoutedEventArgs e)
    {
        isEditing = !isEditing;
        DgOrder.IsReadOnly = !isEditing;
        DgOrder.Background = isEditing
            ? System.Windows.Media.Brushes.LightYellow
            : System.Windows.Media.Brushes.White;
        BtnEditQty.Content = isEditing ? "Done Editing" : "Edit Order";
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
                double sub = OrderItems.Sum(item => item.Total);
                lblCalc.Text = $"Calculated discount: {sub * pct / 100.0:0.00}";
            }
            else
                lblCalc.Text = "Calculated discount: 0.00";
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
            else
            {
                MessageBox.Show("Please enter a value between 0 and 100.", "Invalid",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
        double subtotalVal = OrderItems.Sum(item => item.Total);
        double totalVal = subtotalVal - (subtotalVal * discountPercent / 100.0);
        var payWindow = new PaymentWindow(totalVal, discountPercent)
        {
            Owner = this
        };
        payWindow.ShowDialog();
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void RefreshTotals()
    {
        double subtotalVal = OrderItems.Sum(item => item.Total);
        double discountAmount = subtotalVal * discountPercent / 100.0;
        double totalVal = subtotalVal - discountAmount;
        TxtSubtotal.Text = $"{subtotalVal:0.00}";
        TxtDiscount.Text = discountPercent > 0
            ? $"-{discountAmount:0.00} ({discountPercent}%)"
            : "0.00";
        TxtTotal.Text = $"${totalVal:0.00}";
    }
}
public class OrderItem
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public double Price { get; set; }
    public double Total => Quantity * Price;
    public string PriceDisplay => $"{Price:0.00}";
    public string TotalDisplay => $"{Total:0.00}";
}