using System.Windows;
using Microsoft.Data.SqlClient;
namespace BillingAndPayment;
public partial class RevenueWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public RevenueWindow()
    {
        InitializeComponent();
        DatePicker.SelectedDate = DateTime.Today;
        LoadData();
    }
    private void BtnLoad_Click(object sender, RoutedEventArgs e) => LoadData();
    private void LoadData()
    {
        if (DatePicker.SelectedDate == null) return;
        var date = DatePicker.SelectedDate.Value;
        var orders = new List<OrderSummary>();
        double totalRevenue = 0;
        double totalDiscount = 0;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT o.Id, o.OrderDate, o.Total, o.DiscountPercent,
                         ISNULL(t.TableNumber, 0) AS TableNumber
                  FROM Orders o
                  LEFT JOIN Tables t ON o.TableId = t.Id
                  WHERE CAST(o.OrderDate AS DATE) = @date
                  ORDER BY o.OrderDate", conn);
            cmd.Parameters.AddWithValue("@date", date);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int id = Convert.ToInt32(rdr["Id"]);
                string time = Convert.ToDateTime(rdr["OrderDate"]).ToString("HH:mm");
                int tableNum = Convert.ToInt32(rdr["TableNumber"]);
                double total = Convert.ToDouble(rdr["Total"]);
                double disc = Convert.ToDouble(rdr["DiscountPercent"]);
                orders.Add(new OrderSummary
                {
                    OrderId = id,
                    Time = time,
                    TableDisplay = tableNum > 0 ? $"Table {tableNum}" : "Takeaway",
                    Subtotal = total,
                    DiscountPercent = disc,
                    Total = total,
                });
                totalRevenue += total;
                totalDiscount += disc;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading revenue: " + ex.Message);
        }
        DgOrders.ItemsSource = orders;
        TxtOrderCount.Text = orders.Count.ToString();
        TxtTotalRevenue.Text = $"{totalRevenue:0.00} RON";
        TxtAvgDiscount.Text = orders.Count > 0
            ? $"{(totalDiscount / orders.Count):0.0}%"
            : "0%";
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
public class OrderSummary
{
    public int OrderId { get; set; }
    public string Time { get; set; } = "";
    public string TableDisplay { get; set; } = "";
    public double Subtotal { get; set; }
    public double DiscountPercent { get; set; }
    public double Total { get; set; }
    public string DiscountDisplay => DiscountPercent > 0 ? $"-{Subtotal * DiscountPercent / 100:0.00} ({DiscountPercent}%)" : "0.00";
    public string TotalDisplay => $"{Total:0.00} RON";
}