using System.Windows;
namespace BengosApp;
public partial class AdminDashboard : Window
{
    private readonly string user;
    public AdminDashboard(string username)
    {
        InitializeComponent();
        user = username;
        TxtWelcome.Text = $"Welcome, {username}";
    }
    private void BtnInventory_Click(object sender, RoutedEventArgs e)
    {
        new Inventory.MainWindow().ShowDialog();
    }
    private void BtnStaff_Click(object sender, RoutedEventArgs e)
    {
        var vm = new StaffManagement.StaffViewModel(user, "Admin");
        new StaffManagement.MainWindow(vm).ShowDialog();
    }
   
private void BtnRevenue_Click(object sender, RoutedEventArgs e)
    {
        new BillingAndPayment.RevenueWindow().ShowDialog();
    }
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void BtnLowStock_Click(object sender, RoutedEventArgs e)
    {
        new Inventory.LowStockWindow().ShowDialog();
    }
    private void BtnPrices_Click(object sender, RoutedEventArgs e)
    {
        new Inventory.PriceWindow().ShowDialog();
    }
    
    private void BtnHours_Click(object sender, RoutedEventArgs e)
        => new StaffManagement.HoursWindow().ShowDialog();
    private void BtnSalary_Click(object sender, RoutedEventArgs e)
        => new StaffManagement.SalaryWindow().ShowDialog();
}