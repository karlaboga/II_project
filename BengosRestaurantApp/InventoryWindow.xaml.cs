using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BengosRestaurantApp
{
    public partial class InventoryWindow : Window
    {
        public ObservableCollection<Product> Inventory { get; set; }

        public InventoryWindow()
        {
            InitializeComponent();
            Inventory = new ObservableCollection<Product>();
            DgInventory.ItemsSource = Inventory;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtProduct.Text) ||
                CmbCategory.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtQuantity.Text) ||
                string.IsNullOrWhiteSpace(TxtUnit.Text) ||
                string.IsNullOrWhiteSpace(TxtMinStock.Text))
            {
                MessageBox.Show("Please fill in all fields before adding!", "Missing Information",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0 ||
                !int.TryParse(TxtMinStock.Text, out int minStock) || minStock <= 0)
            {
                MessageBox.Show("Please enter valid numbers for Quantity and Min Stock!", "Invalid Input",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newProd = new Product
            {
                ID = Inventory.Count + 1,
                ProductName = TxtProduct.Text,
                Category = ((ComboBoxItem)CmbCategory.SelectedItem).Content.ToString(),
                Quantity = quantity,
                Unit = TxtUnit.Text,
                Min_Stock = minStock
            };

            Inventory.Add(newProd);
            ClearInputs();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var itemToRemove = Inventory.FirstOrDefault(p => p.ProductName == TxtProduct.Text);

            if (itemToRemove != null)
            {
                Inventory.Remove(itemToRemove);
            }
            else
            {
                MessageBox.Show("Product not found!");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            string editProd = TxtProduct.Text;

            if (string.IsNullOrWhiteSpace(editProd))
            {
                MessageBox.Show("Please enter the name of the product you wish to edit.");
                return;
            }

            bool found = false;
            foreach (Product p in Inventory)
            {
                if (p.ProductName.Equals(editProd, StringComparison.OrdinalIgnoreCase))
                {
                    if (CmbCategory.SelectedItem != null)
                    {
                        p.Category = ((ComboBoxItem)CmbCategory.SelectedItem).Content.ToString();
                    }

                    if (int.TryParse(TxtQuantity.Text, out int qty) && qty > 0)
                    {
                        p.Quantity = qty;
                    }

                    if (!string.IsNullOrWhiteSpace(TxtUnit.Text))
                    {
                        p.Unit = TxtUnit.Text;
                    }

                    if (int.TryParse(TxtMinStock.Text, out int minStock) && minStock > 0)
                    {
                        p.Min_Stock = minStock;
                    }

                    found = true;
                    break;
                }
            }

            if (found)
            {
                DgInventory.Items.Refresh();
                MessageBox.Show("Product updated successfully!");
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Product not found. To edit, the Name must match exactly.");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearInputs()
        {
            TxtProduct.Clear();
            CmbCategory.SelectedIndex = -1;
            TxtQuantity.Clear();
            TxtUnit.Clear();
            TxtMinStock.Clear();
            TxtProduct.Focus();
        }
    }

    public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public int Min_Stock { get; set; }
    }
}
