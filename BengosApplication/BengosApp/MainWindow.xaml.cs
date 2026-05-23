using System.Windows;
using System.Diagnostics;
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
            // 1. Check if the user is an Admin
            if (login.LoggedInRole == "Admin")
            {
                new AdminDashboard(login.LoggedInUser).ShowDialog();
            }
            // 2. Check if the user is a Cook 
            else if (login.LoggedInRole == "Cook")
            {
                // If CookDashboard requires the username, pass it here: login.LoggedInUser
                new CookDashboard().ShowDialog();
            }
            // 3. Anyone else ('Employee' / Waiters) goes here
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
    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}