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
            if (login.LoggedInRole == "Admin")
            {
                new AdminDashboard(login.LoggedInUser).ShowDialog();
            }
            else if (login.LoggedInRole == "Cook")
            {
                new CookDashboard().ShowDialog();
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
    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}
   