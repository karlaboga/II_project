using System.Windows;

using System.Diagnostics;
using System.Windows;
namespace BengosApp;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        var login = new StaffManagement.LoginWindow();
        if (login.ShowDialog() == true)
        {
            if (login.LoggedInRole == "Admin")
            {
                new AdminDashboard(login.LoggedInUser).ShowDialog();
            }
            else
            {
                new BillingAndPayment.TableWindow().ShowDialog();
            }
        }
    }
    private void BtnViewMenu_Click(object sender, RoutedEventArgs e)
    {
        new MenuViewer().ShowDialog();
    }
}