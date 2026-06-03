using System.Windows;
using Microsoft.Data.SqlClient;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Windows.Threading;

namespace BillingAndPayment;

public partial class RevenueWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

    private static readonly SKColor[] Palette =
    {
        SKColor.Parse("#FFFFFF"),
        SKColor.Parse("#4A2D1C"),
        SKColor.Parse("#905327"),
        SKColor.Parse("#C9A3B5"),
        SKColor.Parse("#A0847A"),
        SKColor.Parse("#7A5A68"),
    };

    private DispatcherTimer? refreshTimer;

    public RevenueWindow()
    {
        InitializeComponent();
        FromDatePicker.SelectedDate = DateTime.Today;
        ToDatePicker.SelectedDate = DateTime.Today;
        LoadData();
        StartAutoRefresh();
    }

    private void StartAutoRefresh()
    {
        refreshTimer = new DispatcherTimer();
        refreshTimer.Interval = TimeSpan.FromSeconds(30);
        refreshTimer.Tick += (s, e) => LoadData();
        refreshTimer.Start();
    }

    private void BtnLoad_Click(object sender, RoutedEventArgs e) => LoadData();

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        refreshTimer?.Stop();
        Close();
    }

    private void LoadData()
    {
        if (FromDatePicker.SelectedDate == null || ToDatePicker.SelectedDate == null) return;
        var from = FromDatePicker.SelectedDate.Value;
        var to = ToDatePicker.SelectedDate.Value;
        LoadSummary(from, to);
        LoadCategoryChart(from, to);
        LoadPodium(from, to);
        LoadAllProductsByQty(from, to);
    }

    private void LoadSummary(DateTime from, DateTime to)
    {
        int orderCount = 0;
        double totalRevenue = 0, totalDiscount = 0;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT COUNT(*) AS Cnt,
                         ISNULL(SUM(Total), 0) AS Rev,
                         ISNULL(SUM(Total * DiscountPercent / 100.0), 0) AS TotalDisc
                  FROM Orders
                  WHERE CAST(OrderDate AS DATE) BETWEEN @from AND @to", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                orderCount = Convert.ToInt32(rdr["Cnt"]);
                totalRevenue = Convert.ToDouble(rdr["Rev"]);
                totalDiscount = Convert.ToDouble(rdr["TotalDisc"]);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading summary: " + ex.Message);
        }
        TxtOrderCount.Text = orderCount.ToString();
        TxtTotalRevenue.Text = $"{totalRevenue:0.00} RON";
        TxtAvgDiscount.Text = $"{totalDiscount:0.00} RON";
    }

    private void LoadCategoryChart(DateTime from, DateTime to)
    {
        var names = new List<string>();
        var values = new List<double>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT d.Category, SUM(oi.Quantity * oi.Price) AS Revenue
                  FROM OrderItems oi
                  JOIN Dishes d ON oi.Name = d.Name
                  JOIN Orders o ON oi.OrderId = o.Id
                  WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                  GROUP BY d.Category
                  ORDER BY Revenue DESC", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                names.Add(Convert.ToString(rdr["Category"])!);
                values.Add(Convert.ToDouble(rdr["Revenue"]));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading categories: " + ex.Message);
        }

        var series = new List<ISeries>();
        for (int i = 0; i < names.Count; i++)
        {
            series.Add(new PieSeries<double>
            {
                Values = new double[] { values[i] },
                Name = names[i],
                Fill = new SolidColorPaint(Palette[i % Palette.Length]),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsSize = 12,
            });
        }
        PieCategories.Series = series;
    }

    private void LoadPodium(DateTime from, DateTime to)
    {
        var podiums = new List<DishRevenue>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT TOP 3 d.Name, SUM(oi.Quantity) AS Quantity,
                         SUM(oi.Quantity * oi.Price) AS Revenue
                  FROM OrderItems oi
                  JOIN Dishes d ON oi.Name = d.Name
                  JOIN Orders o ON oi.OrderId = o.Id
                  WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                  GROUP BY d.Name
                  ORDER BY Quantity DESC", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                podiums.Add(new DishRevenue
                {
                    DishName = Convert.ToString(rdr["Name"])!,
                    Quantity = Convert.ToInt32(rdr["Quantity"]),
                    Revenue = Convert.ToDouble(rdr["Revenue"]),
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading podium: " + ex.Message);
        }

        TxtPodium1.Text = podiums.Count > 0 ? podiums[0].DishName : "—";
        TxtPodium1Qty.Text = podiums.Count > 0 ? $"{podiums[0].Quantity} buc" : "";
        TxtPodium2.Text = podiums.Count > 1 ? podiums[1].DishName : "—";
        TxtPodium2Qty.Text = podiums.Count > 1 ? $"{podiums[1].Quantity} buc" : "";
        TxtPodium3.Text = podiums.Count > 2 ? podiums[2].DishName : "—";
        TxtPodium3Qty.Text = podiums.Count > 2 ? $"{podiums[2].Quantity} buc" : "";
    }

    private void LoadAllProductsByQty(DateTime from, DateTime to)
    {
        var dishes = new List<DishRevenue>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT d.Name, SUM(oi.Quantity) AS Quantity,
                         SUM(oi.Quantity * oi.Price) AS Revenue
                  FROM OrderItems oi
                  JOIN Dishes d ON oi.Name = d.Name
                  JOIN Orders o ON oi.OrderId = o.Id
                  WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                  GROUP BY d.Name
                  ORDER BY Quantity DESC", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            int rank = 1;
            while (rdr.Read())
            {
                dishes.Add(new DishRevenue
                {
                    Rank = rank++,
                    DishName = Convert.ToString(rdr["Name"])!,
                    Quantity = Convert.ToInt32(rdr["Quantity"]),
                    Revenue = Convert.ToDouble(rdr["Revenue"]),
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading products: " + ex.Message);
        }
        DgProductsByQty.ItemsSource = dishes;
    }
}

public class DishRevenue
{
    public int Rank { get; set; }
    public string DishName { get; set; } = "";
    public int Quantity { get; set; }
    public double Revenue { get; set; }
    public string RevenueDisplay => $"{Revenue:0.00} RON";
}