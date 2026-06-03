using BillingAndPayment.Models;
using Bengos.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kitchen
{
    public partial class CookBook : Window
    {
        private readonly string connectionString = "Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
        private List<Dish> allDishes = new List<Dish>();

        public CookBook()
        {
            InitializeComponent();
            RecipeListBox.DisplayMemberPath = "Name";
            RecipeListBox.SelectionChanged += RecipeListBox_SelectionChanged;
            LoadDishesFromDatabase();
        }

        private void LoadDishesFromDatabase()
        {
            try
            {
                allDishes.Clear();
                string query = "SELECT Id, Name, Category, PreparationTime, Alergies, Steps FROM dbo.Dishes";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allDishes.Add(new Dish
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Category = reader["Category"].ToString(),
                                PreparationTime = reader["PreparationTime"] != DBNull.Value ? reader["PreparationTime"].ToString() : "--",
                                Alergies = reader["Alergies"] != DBNull.Value ? reader["Alergies"].ToString() : "None",
                                Steps = reader["Steps"] != DBNull.Value ? reader["Steps"].ToString() : "No instructions provided."
                            });
                        }
                    }
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la încărcarea preparatelor: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (CategoryFilterComboBox == null || SearchTextBox == null || RecipeListBox == null) return;

            string selectedCategory = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string searchText = SearchTextBox.Text.Trim().ToLower();

            List<Dish> filteredList = allDishes;

            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "All Dishes")
            {
                filteredList = filteredList.FindAll(d => d.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                filteredList = filteredList.FindAll(d => d.Name.ToLower().Contains(searchText));
            }

            RecipeListBox.ItemsSource = null;
            RecipeListBox.ItemsSource = filteredList;
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void RecipeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecipeListBox.SelectedItem is Dish selectedDish)
            {
                TxtDishName.Text = selectedDish.Name;
                TxtPrepTime.Text = $"{selectedDish.PreparationTime} min";
                TxtAllergens.Text = selectedDish.Alergies;
                TxtInstructions.Text = !string.IsNullOrEmpty(selectedDish.Steps) ? selectedDish.Steps : "No instructions provided.";

                LoadIngredientsForDish(selectedDish.Id);
            }
        }

        private void LoadIngredientsForDish(int dishId)
        {
            try
            {
                List<string> ingredientsList = new List<string>();
                string query = @"
                    SELECT p.Name, di.QuantityRequired, p.Unit 
                    FROM dbo.DishIngredients di
                    INNER JOIN dbo.Produses p ON di.ProductId = p.Id
                    WHERE di.DishId = @DishId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DishId", dishId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string name = reader["Name"].ToString();
                                string qty = reader["QuantityRequired"].ToString();
                                string unit = reader["Unit"].ToString();
                                ingredientsList.Add($"• {name}: {qty} {unit}");
                            }
                        }
                    }
                }

                if (ingredientsList.Count > 0)
                {
                    TxtIngredients.Text = string.Join(Environment.NewLine, ingredientsList);
                }
                else
                {
                    TxtIngredients.Text = "No ingredients registered for this recipe.";
                }
            }
            catch (Exception ex)
            {
                TxtIngredients.Text = "Error loading ingredients.";
                MessageBox.Show("Eroare ingrediente: " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            AddRecipe addRecipeWindow = new AddRecipe();
            addRecipeWindow.Owner = this;

            if (addRecipeWindow.ShowDialog() == true)
            {
                LoadDishesFromDatabase();
            }
        }

        private void BtnEditRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeListBox.SelectedItem is Dish selectedDish)
            {
                // Aici apelăm al doilea constructor trimitând obiectul selectat ca parametru
                AddRecipe editWindow = new AddRecipe(selectedDish);
                editWindow.Owner = this;

                if (editWindow.ShowDialog() == true)
                {
                    LoadDishesFromDatabase();
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe from the sidebar list first to edit.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipeListBox.SelectedItem is Dish selectedDish)
            {
                var result = MessageBox.Show($"Are you sure you want to completely delete the recipe for '{selectedDish.Name}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var conn = new SqlConnection(connectionString);
                        conn.Open();
                        using var cmd = new SqlCommand("DELETE FROM dbo.Dishes WHERE Id = @Id", conn);
                        cmd.Parameters.AddWithValue("@Id", selectedDish.Id);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Recipe deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Resetare UI
                        TxtDishName.Text = "Select a Recipe";
                        TxtPrepTime.Text = "--";
                        TxtAllergens.Text = "--";
                        TxtIngredients.Text = "No ingredients to display.";
                        TxtInstructions.Text = "Select a dish from the sidebar menu to read instructions.";

                        LoadDishesFromDatabase();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting recipe: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe from the sidebar list first to delete.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}