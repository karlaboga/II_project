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
                       COUNT(s.Id) AS TotalShifts,
                       SUM(CASE WHEN s.ShiftType='morning' THEN 1 ELSE 0 END) AS Morning,
                       SUM(CASE WHEN s.ShiftType='evening' THEN 1 ELSE 0 END) AS Evening,
                       SUM(CASE WHEN s.ShiftType='night' THEN 1 ELSE 0 END) AS Night,
                       SUM(CASE WHEN s.ShiftType='morning' THEN 8
                                WHEN s.ShiftType='evening' THEN 8
                                WHEN s.ShiftType='night' THEN 6 ELSE 0 END) AS TotalHours
                FROM Users u
                LEFT JOIN Shifts s ON u.Id = s.StaffId
                LEFT JOIN Salaries sa ON u.Id = sa.UserId
                GROUP BY u.Username, sa.HourlyRate
                ORDER BY u.Username", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int hours = rdr["TotalHours"] != DBNull.Value ? Convert.ToInt32(rdr["TotalHours"]) : 0;
                double rate = rdr["HourlyRate"] != DBNull.Value ? Convert.ToDouble(rdr["HourlyRate"]) : 0;
                list.Add(new EmployeeSalary
                {
                    Username = rdr["Username"]?.ToString() ?? "",
                    HourlyRate = rate,
                    TotalShifts = rdr["TotalShifts"] != DBNull.Value ? Convert.ToInt32(rdr["TotalShifts"]) : 0,
                    MorningShifts = rdr["Morning"] != DBNull.Value ? Convert.ToInt32(rdr["Morning"]) : 0,
                    EveningShifts = rdr["Evening"] != DBNull.Value ? Convert.ToInt32(rdr["Evening"]) : 0,
                    NightShifts = rdr["Night"] != DBNull.Value ? Convert.ToInt32(rdr["Night"]) : 0,
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
    public int TotalShifts { get; set; }
    public int MorningShifts { get; set; }
    public int EveningShifts { get; set; }
    public int NightShifts { get; set; }
    public int TotalHours { get; set; }
    public double TotalSalary { get; set; }
    public string HourlyRateDisplay => $"{HourlyRate:0.00} RON/h";
    public string TotalSalaryDisplay => $"{TotalSalary:0.00} RON";
}