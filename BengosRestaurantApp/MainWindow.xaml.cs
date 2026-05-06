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
            var window = new MenuWindow();
            window.ShowDialog();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
