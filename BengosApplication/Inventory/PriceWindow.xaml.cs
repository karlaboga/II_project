using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Data.SqlClient;
namespace Inventory;
public partial class PriceWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    private ObservableCollection<DishPrice> dishes = new();
    public PriceWindow()
    {
        InitializeComponent();
        LoadData();
    }
    private void LoadData()
    {
        dishes.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Name, Price, Category FROM Dishes ORDER BY Category, Name", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dishes.Add(new DishPrice
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
            MessageBox.Show("Error: " + ex.Message);
        }
        DgPrices.ItemsSource = dishes;
    }
    private void DgPrices_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
    {
        if (e.Column.Header.ToString() == "Price" && e.EditingElement is System.Windows.Controls.TextBox cell)
        {
            if (double.TryParse(cell.Text, out double val) && val > 0)
            {
                if (e.Row.Item is DishPrice d)
                    d.Price = val;
            }
        }
    }
    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            foreach (var d in dishes)
            {
                using var cmd = new SqlCommand("UPDATE Dishes SET Price=@price WHERE Id=@id", conn);
                cmd.Parameters.AddWithValue("@price", d.Price);
                cmd.Parameters.AddWithValue("@id", d.Id);
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Prices saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error saving: " + ex.Message);
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
    public string PriceDisplay => $"${Price:0.00}";
}