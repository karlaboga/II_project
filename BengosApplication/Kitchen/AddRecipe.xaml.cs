using BillingAndPayment.Models;
using Bengos.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Inventory;

namespace Kitchen
{
    public partial class AddRecipe : Window
    {
        private readonly string connectionString = "Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
        public AddRecipe()
        {
            InitializeComponent();
            GridRecipeIngredients.ItemsSource = selectedIngredients;
            LoadInventoryProducts();
            //LoadAvailableUnits();
        }
        private List<Product> inventoryProducts = new List<Product>();
        private List<string> availableUnits = new List<string>();
        private ObservableCollection<Product> selectedIngredients = new ObservableCollection<Product>();

        //------------------------------------------------------------------------------------------------------------------
        private void LoadInventoryProducts()
        {
            try
            {
                inventoryProducts.Clear();
                string query = "SELECT Id, Name, Unit FROM dbo.Produses ORDER BY Name ASC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inventoryProducts.Add(new Product
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                ProductName = reader["Name"].ToString(),
                                Unit = reader["Unit"].ToString()
                            });
                        }
                    }
                }
                ComboInventoryProducts.ItemsSource = inventoryProducts;
                ComboInventoryProducts.DisplayMemberPath = "ProductName";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory items: " + ex.Message);
            }
        }

        //---------------------------------------------------------------------------------------------------------
        private void LoadAvailableUnits()
        {
            try
            {
                availableUnits.Clear();
                string query = "SELECT DISTINCT Unit FROM dbo.Produses WHERE Unit IS NOT NULL AND Unit != '' ORDER BY Unit ASC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            availableUnits.Add(reader["Unit"].ToString());
                        }
                    }
                }
                ComboIngredientUnit.ItemsSource = availableUnits;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading units: " + ex.Message);
            }
        }

        //---------------------------------------------------------------------------------------------------------
        //SAVE RECIPE AND INGREDIENTS
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) || ComboCategory.SelectedItem == null)
            {
                MessageBox.Show("Please complete the Dish Name and Category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(TxtPrice.Text, out double price) || !int.TryParse(TxtPrepTime.Text, out int prepTime))
            {
                MessageBox.Show("Please enter valid numbers for Price and Prep Time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = TxtName.Text.Trim();
            string category = (ComboCategory.SelectedItem as ComboBoxItem)?.Content.ToString();
            string allergens = TxtAllergens.Text.Trim();
            string description = TxtDescription.Text.Trim();
            string steps = TxtSteps.Text.Trim();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Salvare în tabela Dishes
                    string dishQuery = @"
                        INSERT INTO [dbo].[Dishes] (Name, Price, Category, PreparationTime, Alergies, Steps, Description)
                        OUTPUT INSERTED.Id
                        VALUES (@Name, @Price, @Category, @PrepTime, @Allergens, @Steps, @Description)";

                    int newDishId = 0;
                    using (SqlCommand cmd = new SqlCommand(dishQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@PrepTime", prepTime);
                        cmd.Parameters.AddWithValue("@Allergens", allergens);
                        cmd.Parameters.AddWithValue("@Steps", string.IsNullOrEmpty(steps) ? "No steps provided." : steps);
                        cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);

                        newDishId = (int)cmd.ExecuteScalar();
                    }

                    // 2. Salvare în tabela junction DishIngredients
                    string ingredQuery = "INSERT INTO [dbo].[DishIngredients] (DishId, ProductId, QuantityRequired) VALUES (@DishId, @ProductId, @QuantityRequired)";
                    foreach (var ingred in selectedIngredients)
                    {
                        using (SqlCommand cmd = new SqlCommand(ingredQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@DishId", newDishId);
                            cmd.Parameters.AddWithValue("@ProductId", ingred.Id);
                            cmd.Parameters.AddWithValue("@QuantityRequired", ingred.Quantity);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Recipe and its ingredients saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving to database: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //------------------------------------------------------------------------------------------------------------------
        private void BtnAddIngredientRow_Click(object sender, RoutedEventArgs e)
        {
            if (ComboInventoryProducts.SelectedItem is Product prod)
            {
                if (!double.TryParse(TxtIngredientQty.Text, out double qty) || qty <= 0)
                {
                    MessageBox.Show("Please enter a valid positive quantity.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ComboIngredientUnit.SelectedItem == null)
                {
                    MessageBox.Show("Please select a unit of measurement.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // SCHIMBAT: Am adăugat .Trim() ca să curățăm eventualele spații goale din XAML
                string selectedUnit = (ComboIngredientUnit.SelectedItem as ComboBoxItem)?.Content?.ToString()?.Trim() ?? "";

                // --- SISTEM DE CONVERSIE INTELIGENT PENTRU RECEPTAR ---
                // Grame -> Kilograme
                if (selectedUnit.Equals("Grams", StringComparison.OrdinalIgnoreCase) || selectedUnit.Equals("g", StringComparison.OrdinalIgnoreCase))
                {
                    qty = qty / 1000.0; // 250g devine 0.25 kg
                    selectedUnit = "Kilograms";
                }
                // REPARAT: Schimbat din "Milliliters" în "Mililiters" (cu un singur l) ca să bată perfect cu interfața ta XAML!
                else if (selectedUnit.Equals("Milliliters", StringComparison.OrdinalIgnoreCase) || selectedUnit.Equals("ml", StringComparison.OrdinalIgnoreCase))
                {
                    qty = qty / 1000.0; // 200ml devine 0.2 Liters
                    selectedUnit = "Liters";
                }
                // Bucăți / Unități fixe (Pieces, Packs, etc.) rămân exact la fel, nu se modifică nimic matematic
                // ----------------------------------------------------------------------

                // Verificăm dacă ingredientul este deja adăugat CU ACEEAȘI UNITATE standardizată ca să îi creștem doar cantitatea
                foreach (var existing in selectedIngredients)
                {
                    if (existing.Id == prod.Id && existing.Unit.Equals(selectedUnit, StringComparison.OrdinalIgnoreCase))
                    {
                        existing.Quantity += qty;
                        TxtIngredientQty.Clear();
                        return;
                    }
                }

                // Adăugăm rândul proaspăt convertit în colecția vizuală
                selectedIngredients.Add(new Product
                {
                    Id = prod.Id,
                    ProductName = prod.ProductName,
                    Unit = selectedUnit,
                    Quantity = qty
                });

                TxtIngredientQty.Clear();
            }
            else
            {
                MessageBox.Show("Please select a product from the list first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //------------------------------------------------------------------------------------------------------------------
        private void BtnRemoveIngredientRow_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Product item)
            {
                selectedIngredients.Remove(item);
            }
        }
    }
}