using BillingAndPayment.Models;
using Bengos.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Inventory;

namespace Kitchen
{
    public partial class AddRecipe : Window
    {
        private readonly string connectionString = "Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
        private List<Product> inventoryProducts = new List<Product>();
        private ObservableCollection<Product> selectedIngredients = new ObservableCollection<Product>();

        // Identificator hibrid: 0 = Rețetă Nouă (Insert), >0 = Editare Rețetă (Update)
        private int editingDishId = 0;

        // CONSTRUCTOR 1: Folosit la crearea unei rețete noi
        public AddRecipe()
        {
            InitializeComponent();
            GridRecipeIngredients.ItemsSource = selectedIngredients;
            LoadInventoryProducts();
        }

        // CONSTRUCTOR 2: Apelat automat la editare din CookBook
        public AddRecipe(Dish dishToEdit) : this()
        {
            editingDishId = dishToEdit.Id;

            // Populare date existente în UI
            TxtName.Text = dishToEdit.Name;
            TxtPrice.Text = "";
            TxtPrepTime.Text = dishToEdit.PreparationTime.Replace(" min", "").Trim();
            TxtAllergens.Text = dishToEdit.Alergies;
            TxtSteps.Text = dishToEdit.Steps;

            // Selectare categorie corespunzătoare
            foreach (ComboBoxItem item in ComboCategory.Items)
            {
                if (item.Content.ToString().Equals(dishToEdit.Category, StringComparison.OrdinalIgnoreCase))
                {
                    ComboCategory.SelectedItem = item;
                    break;
                }
            }

            // Schimbare text buton de acțiune
            BtnSave.Content = "💾 Update Recipe";

            // Încărcare ingrediente în tabelul de lucru local
            LoadIngredientsForEditing(dishToEdit.Id);
        }

        private void LoadIngredientsForEditing(int dishId)
        {
            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();
                string query = @"
                    SELECT p.Id, p.Name, di.QuantityRequired, p.Unit 
                    FROM dbo.DishIngredients di
                    INNER JOIN dbo.Produses p ON di.ProductId = p.Id
                    WHERE di.DishId = @DishId";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DishId", dishId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    selectedIngredients.Add(new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        ProductName = reader["Name"].ToString(),
                        Unit = reader["Unit"].ToString(),
                        Quantity = Convert.ToDouble(reader["QuantityRequired"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading existing ingredients: " + ex.Message);
            }
        }

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

        // --- NOU: FILTRAREA DINAMICĂ A UNITĂȚILOR DE MĂSURĂ ---
        private void ComboInventoryProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboInventoryProducts.SelectedItem is Product selectedProd)
            {
                List<string> allowedUnits = new List<string>();
                string baseUnit = selectedProd.Unit?.Trim();

                // Verificăm unitatea de bază din baza de date (Produses -> Unit)
                if (string.Equals(baseUnit, "Kilograms", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(baseUnit, "KG", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(baseUnit, "Grams", StringComparison.OrdinalIgnoreCase))
                {
                    allowedUnits.Add("Grams");
                    allowedUnits.Add("Kilograms");
                }
                else if (string.Equals(baseUnit, "Liters", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(baseUnit, "L", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(baseUnit, "Mililiters", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(baseUnit, "Milliliters", StringComparison.OrdinalIgnoreCase))
                {
                    allowedUnits.Add("Mililiters");
                    allowedUnits.Add("Liters");
                }
                else
                {
                    // Pentru Pieces, Packs, Units sau orice altceva nespecificat
                    allowedUnits.Add(string.IsNullOrEmpty(baseUnit) ? "Pieces" : baseUnit);
                }

                ComboIngredientUnit.ItemsSource = allowedUnits;
                ComboIngredientUnit.SelectedIndex = 0; // Selectează automat prima opțiune disponibilă
            }
            else
            {
                ComboIngredientUnit.ItemsSource = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validare Nume și Categorie
            if (string.IsNullOrWhiteSpace(TxtName.Text) || ComboCategory.SelectedItem == null)
            {
                MessageBox.Show("Please complete the Dish Name and Category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. REPARAT: Validare strictă pentru preț (Să fie număr și strict mai mare ca 0)
            if (!double.TryParse(TxtPrice.Text, out double price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0 RON.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtPrepTime.Text, out int prepTime)) prepTime = 0;

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
                    int targetDishId = editingDishId;

                    if (editingDishId == 0)
                    {
                        // AZUL A: INSERARE REȚETĂ NOUĂ
                        string dishQuery = @"
                            INSERT INTO [dbo].[Dishes] (Name, Price, Category, PreparationTime, Alergies, Steps, Description)
                            OUTPUT INSERTED.Id
                            VALUES (@Name, @Price, @Category, @PrepTime, @Allergens, @Steps, @Description)";

                        using var cmd = new SqlCommand(dishQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Price", price);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@PrepTime", prepTime);
                        cmd.Parameters.AddWithValue("@Allergens", allergens);
                        cmd.Parameters.AddWithValue("@Steps", string.IsNullOrEmpty(steps) ? "No steps provided." : steps);
                        cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);

                        targetDishId = (int)cmd.ExecuteScalar();
                    }
                    else
                    {
                        // CAZUL B: ACTUALIZARE REȚETĂ EXISTENTĂ
                        string updateDishQuery = @"
                            UPDATE [dbo].[Dishes] 
                            SET Name=@Name, Category=@Category, PreparationTime=@PrepTime, Alergies=@Allergens, Steps=@Steps
                            WHERE Id=@DishId";

                        using var cmdUpdate = new SqlCommand(updateDishQuery, conn, transaction);
                        cmdUpdate.Parameters.AddWithValue("@DishId", targetDishId);
                        cmdUpdate.Parameters.AddWithValue("@Name", name);
                        cmdUpdate.Parameters.AddWithValue("@Category", category);
                        cmdUpdate.Parameters.AddWithValue("@PrepTime", prepTime);
                        cmdUpdate.Parameters.AddWithValue("@Allergens", allergens);
                        cmdUpdate.Parameters.AddWithValue("@Steps", steps);
                        cmdUpdate.ExecuteNonQuery();

                        // Ștergem vechile asocieri din tabela junction pentru rescriere curată
                        using var cmdClean = new SqlCommand("DELETE FROM [dbo].[DishIngredients] WHERE DishId=@DishId", conn, transaction);
                        cmdClean.Parameters.AddWithValue("@DishId", targetDishId);
                        cmdClean.ExecuteNonQuery();
                    }

                    // Scriere / Rescriere ingrediente în tabela junction
                    string ingredQuery = "INSERT INTO [dbo].[DishIngredients] (DishId, ProductId, QuantityRequired) VALUES (@DishId, @ProductId, @QuantityRequired)";
                    foreach (var ingred in selectedIngredients)
                    {
                        using var cmdIngred = new SqlCommand(ingredQuery, conn, transaction);
                        cmdIngred.Parameters.AddWithValue("@DishId", targetDishId);
                        cmdIngred.Parameters.AddWithValue("@ProductId", ingred.Id);
                        cmdIngred.Parameters.AddWithValue("@QuantityRequired", ingred.Quantity);
                        cmdIngred.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Recipe saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving data: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

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

                // MODIFICAT: Deoarece acum legăm o listă simplă de string-uri în ComboBox,
                // citim direct textul selectat, fără conversie prin ComboBoxItem.
                string selectedUnit = ComboIngredientUnit.SelectedItem?.ToString()?.Trim() ?? "";

                if (selectedUnit.Equals("Grams", StringComparison.OrdinalIgnoreCase) || selectedUnit.Equals("g", StringComparison.OrdinalIgnoreCase))
                {
                    qty = qty / 1000.0;
                    selectedUnit = "Kilograms";
                }
                else if (selectedUnit.Equals("Mililiters", StringComparison.OrdinalIgnoreCase) || selectedUnit.Equals("ml", StringComparison.OrdinalIgnoreCase))
                {
                    qty = qty / 1000.0;
                    selectedUnit = "Liters";
                }

                foreach (var existing in selectedIngredients)
                {
                    if (existing.Id == prod.Id && existing.Unit.Equals(selectedUnit, StringComparison.OrdinalIgnoreCase))
                    {
                        existing.Quantity += qty;
                        TxtIngredientQty.Clear();
                        return;
                    }
                }

                selectedIngredients.Add(new Product
                {
                    Id = prod.Id,
                    ProductName = prod.ProductName,
                    Unit = selectedUnit,
                    Quantity = qty
                });

                TxtIngredientQty.Clear();
            }
        }

        private void BtnRemoveIngredientRow_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Product item)
            {
                selectedIngredients.Remove(item);
            }
        }
    }
}