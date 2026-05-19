using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace BengosApp;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void Card_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Border b)
        {
            b.Background = System.Windows.Media.Brushes.White;
            b.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                BlurRadius = 15,
                ShadowDepth = 2,
                Opacity = 0.2,
                Color = System.Windows.Media.Color.FromRgb(0x5C, 0x3A, 0x21)
            };
        }
    }
    private void Card_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Border b)
        {
            b.Background = System.Windows.Media.Brushes.Transparent;
            b.Effect = null;
        }
    }
    private void BtnInventory_Click(object sender, RoutedEventArgs e)
    {
        new Inventory.MainWindow().ShowDialog();
    }
    private void BtnStaff_Click(object sender, RoutedEventArgs e)
    {
        var login = new StaffManagement.LoginWindow();
        if (login.ShowDialog() == true)
        {
            var vm = new StaffManagement.StaffViewModel(login.LoggedInUser, login.LoggedInRole);
            new StaffManagement.MainWindow(vm).ShowDialog();
        }
    }
    private void BtnBilling_Click(object sender, RoutedEventArgs e)
    {
        new BillingAndPayment.BillingWindow().ShowDialog();
    }
    private void BtnMenu_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://localhost:5000",
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show(
                "Open the BengosMenu project and run it.\nIt should be on http://localhost:5000",
                "Web Menu", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}