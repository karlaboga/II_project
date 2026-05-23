using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BengosApp
{
    /// <summary>
    /// Interaction logic for CookDashboard.xaml
    /// </summary>
    public partial class CookDashboard : Window
    {
        public CookDashboard()
        {
            InitializeComponent();
        }

        private void CookBookButton_Click(object sender, RoutedEventArgs e)
        {

            var cookbookWindow = new CookBook();
            cookbookWindow.Show();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ActiveOrders activeOrdersWindow = new ActiveOrders();
            activeOrdersWindow.Show();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
