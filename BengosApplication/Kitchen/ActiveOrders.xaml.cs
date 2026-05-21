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

namespace Kitchen
{
    /// <summary>
    /// Interaction logic for ActiveOrders.xaml
    /// </summary>
    public partial class ActiveOrders : Window
    {
        public ActiveOrders()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            // Verificăm dacă butonul a fost apăsat și dacă are un context de date
            if (sender is Button btn && btn.DataContext != null)
            {
                // Aici va fi logica ta de procesare (ex: ștergerea din lista de comenzi)
                MessageBox.Show("Comanda a fost marcată ca finalizată.");
            }
        }
    }
}