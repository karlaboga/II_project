using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Inventory
{
    public partial class PriceWindow : Window
    {
        private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

        // Colectia legata direct de interfata grafica
        private ObservableCollection<DishPrice> dishesInView = new();

        // Cache local general folosit pentru filtrare ultra-rapida fara spam pe Azure
        private List<DishPrice> allDishesCache = new();

        public PriceWindow()
        {
            InitializeComponent();
            DgPrices.ItemsSource = dishesInView;
            LoadDataFromDatabase();

            // Setam filtrul initial pe "All"
            ComboFilterCategory.SelectedIndex = 0;
        }

        private void LoadDataFromDatabase()
        {
            allDishesCache.Clear();
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();
                using var cmd = new SqlCommand("SELECT Id, Name, Price, Category FROM Dishes ORDER BY Category, Name", conn);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    allDishesCache.Add(new DishPrice
                    {
                        Id = Convert.ToInt32(rdr["Id"]),
                        Name = rdr["Name"]?.ToString() ?? "",
                        Category = rdr["Category"]?.ToString() ?? "",
                        Price = Convert.ToDouble(rdr["Price"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ApplyCategoryFilter();
        }

        private void ApplyCategoryFilter()
        {
            if (ComboFilterCategory == null) return;

            string selectedCategory = (ComboFilterCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "All";

            dishesInView.Clear();

            var filtered = selectedCategory == "All"
                ? allDishesCache
                : allDishesCache.Where(d => d.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
            {
                dishesInView.Add(item);
            }
        }

        private void ComboFilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyCategoryFilter();
        }

        private void DgPrices_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Verificam daca celula editata este cea de pret
            if (e.Column.Header.ToString() == "Price" && e.EditingElement is TextBox cell)
            {
                // Curatam textul introdus de eventuale caractere reziduale " RON" introduse din greseala de user
                string cleanText = cell.Text.Replace("RON", "").Replace("ron", "").Trim();

                if (double.TryParse(cleanText, out double parsedValue) && parsedValue >= 0)
                {
                    if (e.Row.Item is DishPrice dish)
                    {
                        dish.Price = parsedValue;

                        // Actualizam si valoarea din cache-ul global in caz ca userul schimba categoria mai tarziu
                        var cacheItem = allDishesCache.FirstOrDefault(x => x.Id == dish.Id);
                        if (cacheItem != null)
                        {
                            cacheItem.Price = parsedValue;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid positive number for the price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true; // Opreste parasirea celulei pana cand valoarea este corectata
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();

                // Salvam absolut tot ce a fost incarcat in cache-ul local (modificate sau nu)
                foreach (var d in allDishesCache)
                {
                    using var cmd = new SqlCommand("UPDATE Dishes SET Price=@price WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@price", d.Price);
                    cmd.Parameters.AddWithValue("@id", d.Id);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("All changes to dish prices have been saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataFromDatabase(); // Reincarcare curata
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes to database: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
    }

    public class DishPrice
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public double Price { get; set; }
    }
}