using System.Windows;
using Microsoft.Data.SqlClient;
namespace StaffManagement;
public partial class SalaryWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public SalaryWindow()
    {
        InitializeComponent();
        LoadData();
    }
    private void LoadData()
    {
        var list = new List<EmployeeSalary>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT u.Username, sa.HourlyRate,
                       SUM(CASE WHEN s.ShiftType='morning' THEN 8
                                WHEN s.ShiftType='evening' THEN 8
                                WHEN s.ShiftType='night' THEN 6 ELSE 0 END) AS TotalHours
                FROM Shifts s
                JOIN Users u ON s.StaffId = u.Id
                JOIN Salaries sa ON u.Id = sa.UserId
                GROUP BY u.Username, sa.HourlyRate
                ORDER BY u.Username", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int hours = Convert.ToInt32(rdr["TotalHours"]);
                double rate = Convert.ToDouble(rdr["HourlyRate"]);
                list.Add(new EmployeeSalary
                {
                    Username = rdr["Username"]?.ToString() ?? "",
                    HourlyRate = rate,
                    TotalHours = hours,
                    TotalSalary = hours * rate
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        DgSalary.ItemsSource = list;
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
public class EmployeeSalary
{
    public string Username { get; set; } = "";
    public double HourlyRate { get; set; }
    public int TotalHours { get; set; }
    public double TotalSalary { get; set; }
    public string HourlyRateDisplay => $"{HourlyRate:0.00} RON/h";
    public string TotalSalaryDisplay => $"{TotalSalary:0.00} RON";
}