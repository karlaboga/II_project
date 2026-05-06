using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.SqlClient;

namespace inventoryWPF
{
    public partial class MainWindow : Window
    {
        // ObservableCollection handles UI updates automatically in WPF
        //string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\_fac\an3\sem2\II\II_proj\inventoryWPF\inventoryWPF\Database_Products.mdf;Integrated Security=True";
        
        string connString;

        ObservableCollection<Product> inventory = new ObservableCollection<Product>();

        public MainWindow()
        {
            InitializeComponent();
            // 2.Build the path and the connection string here
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database_Products.mdf");
            connString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True";

            dataGrid1.ItemsSource = inventory;
            LoadData(); // Initial load
        }
        private void comboBox_Unit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Logic can go here if needed later
        }

        private void LoadData()
        {
            try
            {
                inventory.Clear();
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "SELECT * FROM Products";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        inventory.Add(new Product
                        {
                            ID = Convert.ToInt32(reader["Id"]),
                            ProductName = reader["ProductName"].ToString(),
                            Category = reader["Category"].ToString(),
                            Quantity = Convert.ToInt32(reader["Quantity"]),
                            Unit = reader["Unit"].ToString(),
                            Min_Stock = Convert.ToInt32(reader["Min_Stock"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Load Error: " + ex.Message);
            }
        }
        private void button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(textBox_IDprod.Text, out int id)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "INSERT INTO Products (Id, ProductName, Category, Quantity, Unit, Min_Stock) " +
                                   "VALUES (@id, @name, @cat, @qty, @unit, @min)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", textBox_Product.Text);
                    cmd.Parameters.AddWithValue("@cat", comboBox_Category.Text);
                    cmd.Parameters.AddWithValue("@qty", int.Parse(textBox_Quantity.Text));
                    cmd.Parameters.AddWithValue("@unit", comboBox_Unit.Text);
                    cmd.Parameters.AddWithValue("@min", int.Parse(textBox_MinStock.Text));

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadData();
                ClearInputs();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(textBox_IDprod.Text, out int id)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "UPDATE Products SET ProductName=@name, Category=@cat, Quantity=@qty, Unit=@unit, Min_Stock=@min WHERE Id=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", textBox_Product.Text);
                    cmd.Parameters.AddWithValue("@cat", comboBox_Category.Text);
                    cmd.Parameters.AddWithValue("@qty", int.Parse(textBox_Quantity.Text));
                    cmd.Parameters.AddWithValue("@unit", comboBox_Unit.Text);
                    cmd.Parameters.AddWithValue("@min", int.Parse(textBox_MinStock.Text));

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0) MessageBox.Show("Product Updated!");
                }
                LoadData();
                ClearInputs();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(textBox_IDprod.Text, out int id)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    string query = "DELETE FROM Products WHERE Id=@id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadData();
                ClearInputs();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ClearInputs()
        {
            textBox_IDprod.Clear();
            textBox_Product.Clear();
            comboBox_Unit.SelectedIndex = -1;
            textBox_Quantity.Text = "0";
            textBox_MinStock.Text = "0";
            comboBox_Category.SelectedIndex = -1;
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid1.SelectedItem is Product selectedProduct)
            {
                // Fill the textboxes/comboboxes with the existing data
                textBox_IDprod.Text = selectedProduct.ID.ToString();
                textBox_Product.Text = selectedProduct.ProductName;
                comboBox_Category.Text = selectedProduct.Category;
                textBox_Quantity.Text = selectedProduct.Quantity.ToString();
                comboBox_Unit.Text = selectedProduct.Unit;
                textBox_MinStock.Text = selectedProduct.Min_Stock.ToString();

                // Disable ID editing during an edit to prevent primary key errors
                textBox_IDprod.IsEnabled = false;
            }
        }

        private void ApplyFilters()
        {
            try
            {
                // 1. Get the actual selected item correctly
                if (comboBox_Category.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedCategory = selectedItem.Content.ToString();
                    inventory.Clear();

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        // Base query
                        string query = "SELECT * FROM Products";

                        // Only add the WHERE clause if the user didn't pick "All"
                        if (selectedCategory != "All")
                        {
                            query += " WHERE Category = @cat";
                        }

                        SqlCommand cmd = new SqlCommand(query, conn);

                        if (selectedCategory != "All")
                        {
                            cmd.Parameters.AddWithValue("@cat", selectedCategory);
                        }

                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            inventory.Add(new Product
                            {
                                ID = Convert.ToInt32(reader["Id"]),
                                ProductName = reader["ProductName"].ToString(),
                                Category = reader["Category"].ToString(),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Unit = reader["Unit"].ToString(),
                                Min_Stock = Convert.ToInt32(reader["Min_Stock"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filter Error: " + ex.Message);
            }
        }

        private void FilterControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Every time a dropdown changes, we re-run the filtered query
            ApplyFilters();
        }

        private void button_ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            comboBox_Category.SelectedIndex = -1;
            comboBox_Unit.SelectedIndex = -1;
            LoadData(); // This was your original function that runs "SELECT * FROM Products"
        }
    }
}