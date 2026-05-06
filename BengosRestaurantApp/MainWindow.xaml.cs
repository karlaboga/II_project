using System.Diagnostics;
using System.Windows;

namespace BengosRestaurantApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnInventory_Click(object sender, RoutedEventArgs e)
        {
            var window = new InventoryWindow();
            window.ShowDialog();
        }

        private void BtnStaff_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
            {
                var staffWindow = new StaffWindow(loginWindow.Username, loginWindow.Role);
                staffWindow.ShowDialog();
            }
        }

        private void BtnBilling_Click(object sender, RoutedEventArgs e)
        {
            var window = new BillingWindow();
            window.ShowDialog();
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            // Launch the ASP.NET MVC web app (DigitalClientMenu)
            var webAppPath = System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "..\\..\\..\\..\\DigitalClientMenu\\BengosMenu\\BengosMenu.csproj");

            // Try to open in Visual Studio or run with dotnet
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://localhost:44300", // Default IIS Express port
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Please open the DigitalClientMenu project in Visual Studio and run it.",
                    "Web App Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
