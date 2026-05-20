using System.Windows;
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
                "SELECT Name, Unit, Quantity, MinStock FROM Produses WHERE Quantity < MinStock ORDER BY Quantity ASC", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int qty = Convert.ToInt32(rdr["Quantity"]);
                int min = Convert.ToInt32(rdr["MinStock"]);
                items.Add(new StockItem
                {
                    Name = rdr["Name"]?.ToString() ?? "",
                    Unit = rdr["Unit"]?.ToString() ?? "",
                    Quantity = qty,
                    MinStock = min,
                    Status = qty == 0 ? "OUT OF STOCK" : "LOW"
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        DgLowStock.ItemsSource = items;
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
public class StockItem
{
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public string Status { get; set; } = "";
}