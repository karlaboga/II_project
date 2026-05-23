using System.Windows;
namespace BengosApp;
public partial class EmployeeDashboard : Window
{
    private readonly string user;
    private readonly string role;
    public EmployeeDashboard(string username, string roleName)
    {
        InitializeComponent();
        user = username;
        role = roleName;
       
    }
    private void BtnStaff_Click(object sender, RoutedEventArgs e)
    {
        var vm = new StaffManagement.StaffViewModel(user, role);
        new StaffManagement.MainWindow(vm).ShowDialog();
    }
    private void BtnMenu_Click(object sender, RoutedEventArgs e)
    {
        new MenuViewer().ShowDialog();
    }
    private void BtnTables_Click(object sender, RoutedEventArgs e)
    {
        new BillingAndPayment.TableWindow().ShowDialog();
    }
    private void BtnPayment_Click(object sender, RoutedEventArgs e)
    {
        new BillingAndPayment.BillingWindow().ShowDialog();
    }
    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}