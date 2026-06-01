using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Runtime.CompilerServices; 
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace Inventory;

public partial class LowStockWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

    public LowStockWindow()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        var items = new List<StockItem>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT Id, Name, Category, Unit, Quantity, MinStock FROM Produses WHERE Quantity < MinStock ORDER BY Quantity ASC", conn);

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int qty = Convert.ToInt32(rdr["Quantity"]);
                int min = Convert.ToInt32(rdr["MinStock"]);

                items.Add(new StockItem
                {
                    Id = Convert.ToInt32(rdr["Id"]),
                    Name = rdr["Name"]?.ToString() ?? "",
                    Category = rdr["Category"]?.ToString() ?? "",
                    Unit = rdr["Unit"]?.ToString() ?? "",
                    Quantity = qty,
                    MinStock = min,
                    Status = qty == 0 ? "OUT OF STOCK" : "LOW",
                    AddQuantity = 0
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading data: " + ex.Message);
        }

        DgLowStock.ItemsSource = items;
    }

    private void BtnRestock_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is StockItem selectedItem)
        {
            if (selectedItem.AddQuantity <= 0)
            {
                MessageBox.Show("Introduceți o cantitate mai mare decât 0 pentru reaprovizionare!", "Atenție", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();

                string updateQuery = "UPDATE Produses SET Quantity = Quantity + @addQty WHERE Id = @id";

                using var cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@addQty", selectedItem.AddQuantity);
                cmd.Parameters.AddWithValue("@id", selectedItem.Id);

                cmd.ExecuteNonQuery();

                MessageBox.Show($"Stocul pentru '{selectedItem.Name}' a fost suplimentat cu {selectedItem.AddQuantity} {selectedItem.Unit}!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la salvarea în baza de date: " + ex.Message);
            }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}

public class StockItem : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Unit { get; set; } = "";
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public string Status { get; set; } = "";

    private int _addQuantity;
    public int AddQuantity
    {
        get => _addQuantity;
        set
        {
            _addQuantity = Math.Max(0, value);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}