using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
namespace BengosApp;
public partial class MenuViewer : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    private ObservableCollection<DishItem> allDishes = new();
    public MenuViewer()
    {
        InitializeComponent();
        LoadDishes();
    }
    private void LoadDishes()
    {
        allDishes.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Name, Description, Price, Category, ImageUrl FROM Dishes ORDER BY Category, Name", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                allDishes.Add(new DishItem
                {
                    Name = reader["Name"]?.ToString() ?? "",
                    Description = reader["Description"]?.ToString() ?? "",
                    Price = Convert.ToDouble(reader["Price"]),
                    Category = reader["Category"]?.ToString() ?? "",
                    ImageUrl = reader["ImageUrl"]?.ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading menu: " + ex.Message);
        }
        LstMenu.ItemsSource = allDishes;
    }
    private void Filter_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        string cat = btn?.Content?.ToString() ?? "All";
        if (cat == "All")
            LstMenu.ItemsSource = allDishes;
        else
            LstMenu.ItemsSource = new ObservableCollection<DishItem>(
                allDishes.Where(d => d.Category == cat).ToList());
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
public class DishItem
{
    public bool HasMissingIngredients { get; set; }
    public string WarningText => HasMissingIngredients ? "⚠️ Missing ingredients" : "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public double Price { get; set; }
    public string Category { get; set; } = "";
    public string PriceDisplay => $"{Price:0.00} RON";
    public string ImageUrl { get; set; } = "";

}