using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using SkiaSharp;
using System.Security.AccessControl;
using System.Windows;

namespace BillingAndPayment;

public partial class RevenueWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

    private static readonly SKColor[] Palette =
    {
        SKColor.Parse("#5E3E4A"),
        SKColor.Parse("#C9A3B5"),
        SKColor.Parse("#8A6B7A"),
        SKColor.Parse("#7A5A68"),
        SKColor.Parse("#4B2F3A"),
        SKColor.Parse("#B3957D"),
    };

    public RevenueWindow()
    {
        InitializeComponent();
        FromDatePicker.SelectedDate = DateTime.Today;
        ToDatePicker.SelectedDate = DateTime.Today;
        LoadData();
    }

    private void BtnLoad_Click(object sender, RoutedEventArgs e) => LoadData();

    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void LoadData()
    {
        if (FromDatePicker.SelectedDate == null || ToDatePicker.SelectedDate == null) return;
        var from = FromDatePicker.SelectedDate.Value;
        var to = ToDatePicker.SelectedDate.Value;
        LoadOrders(from, to);
        LoadCategoryChart(from, to);
        LoadDailyTrend(from, to);
        LoadTopDishes(from, to);
        LoadTypeChart(from, to);
    }

    private void LoadOrders(DateTime from, DateTime to)
    {
        var orders = new List<OrderSummary>();
        double totalRevenue = 0, totalDiscount = 0;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT o.Id, o.OrderDate, o.Total, o.DiscountPercent,
                         ISNULL(t.TableNumber, 0) AS TableNumber
                  FROM Orders o
                  LEFT JOIN Tables t ON o.TableId = t.Id
                  WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                  ORDER BY o.OrderDate", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
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
            MessageBox.Show("Error loading orders: " + ex.Message);
        }
        DgOrders.ItemsSource = orders;
        TxtOrderCount.Text = orders.Count.ToString();
        TxtTotalRevenue.Text = $"{totalRevenue:0.00} RON";
        TxtAvgDiscount.Text = orders.Count > 0
            ? $"{(totalDiscount / orders.Count):0.0}%"
            : "0%";
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

    private void LoadDailyTrend(DateTime from, DateTime to)
    {
        var dates = new List<string>();
        var revenues = new List<double>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT CAST(o.OrderDate AS DATE) AS OrderDay,
                  SUM(o.Total) AS Revenue
                  FROM Orders o
                  WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                  GROUP BY CAST(o.OrderDate AS DATE)
                  ORDER BY OrderDay", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dates.Add(Convert.ToDateTime(rdr["OrderDay"]).ToString("MMM dd"));
                revenues.Add(Convert.ToDouble(rdr["Revenue"]));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading daily trend: " + ex.Message);
        }

        ChartDailyTrend.Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = revenues,
                Name = "Revenue",
                Fill = new SolidColorPaint(Palette[0]),
                Stroke = null,
            }
        };
        ChartDailyTrend.XAxes = new Axis[]
        {
            new Axis
            {
                Labels = dates,
                LabelsRotation = 45
            }
        };
        ChartDailyTrend.YAxes = new Axis[]
{
    new Axis
    {
        Name = "RON",
        NameTextSize = 12,
        Labeler = value => value.ToString("N0")
    }

};}

    private void LoadTopDishes(DateTime from, DateTime to)
    {
        var dishes = new List<DishRevenue>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT TOP 10 d.Name, SUM(oi.Quantity) AS Quantity,
                         SUM(oi.Quantity * oi.Price) AS Revenue
                 FROM OrderItems oi
                JOIN Dishes d ON oi.Name = d.Name
                JOIN Orders o ON oi.OrderId = o.Id
                 WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                GROUP BY d.Name
                ORDER BY Revenue DESC", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                dishes.Add(new DishRevenue
                {
                    DishName = Convert.ToString(rdr["Name"])!,
                    Quantity = Convert.ToInt32(rdr["Quantity"]),
                    Revenue = Convert.ToDouble(rdr["Revenue"]),
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading top dishes: " + ex.Message);
        }
        DgTopDishes.ItemsSource = dishes;
    }

    private void LoadTypeChart(DateTime from, DateTime to)
    {
        double dineIn = 0, takeaway = 0;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT CASE WHEN o.TableId IS NOT NULL THEN 'Dine-in' ELSE 'Takeaway' END AS OrderType,
                         SUM(o.Total) AS Revenue
                  FROM Orders o
                  WHERE CAST(o.OrderDate AS DATE) BETWEEN @from AND @to
                  GROUP BY CASE WHEN o.TableId IS NOT NULL THEN 'Dine-in' ELSE 'Takeaway' END", conn);
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                string type = Convert.ToString(rdr["OrderType"])!;
                double rev = Convert.ToDouble(rdr["Revenue"]);
                if (type == "Dine-in") dineIn = rev;
                else takeaway = rev;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading order type breakdown: " + ex.Message);
        }

        PieType.Series = new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new double[] { dineIn },
                Name = "Dine-in",
                Fill = new SolidColorPaint(Palette[0]),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsSize = 13,
            },
            new PieSeries<double>
            {
                Values = new double[] { takeaway },
                Name = "Takeaway",
                Fill = new SolidColorPaint(Palette[1]),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsSize = 13,
            }
        };
    }
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

public class DishRevenue
{
    public string DishName { get; set; } = "";
    public int Quantity { get; set; }
    public double Revenue { get; set; }
    public string RevenueDisplay => $"{Revenue:0.00} RON";
}