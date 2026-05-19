using System.Windows;
using Microsoft.Data.SqlClient;
namespace StaffManagement;
public partial class HoursWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public HoursWindow()
    {
        InitializeComponent();
        LoadData();
    }
    private void LoadData()
    {
        var list = new List<EmployeeHours>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(@"
                SELECT u.Username,
                       COUNT(*) AS TotalShifts,
                       SUM(CASE WHEN s.ShiftType='morning' THEN 1 ELSE 0 END) AS Morning,
                       SUM(CASE WHEN s.ShiftType='evening' THEN 1 ELSE 0 END) AS Evening,
                       SUM(CASE WHEN s.ShiftType='night' THEN 1 ELSE 0 END) AS Night
                FROM Shifts s
                JOIN Users u ON s.StaffId = u.Id
                GROUP BY u.Username
                ORDER BY u.Username", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int mor = Convert.ToInt32(rdr["Morning"]);
                int eve = Convert.ToInt32(rdr["Evening"]);
                int night = Convert.ToInt32(rdr["Night"]);
                list.Add(new EmployeeHours
                {
                    Username = rdr["Username"]?.ToString() ?? "",
                    TotalShifts = Convert.ToInt32(rdr["TotalShifts"]),
                    MorningShifts = mor,
                    EveningShifts = eve,
                    NightShifts = night,
                    TotalHours = (mor * 8) + (eve * 8) + (night * 6)
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
        DgHours.ItemsSource = list;
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}
public class EmployeeHours
{
    public string Username { get; set; } = "";
    public int TotalShifts { get; set; }
    public int MorningShifts { get; set; }
    public int EveningShifts { get; set; }
    public int NightShifts { get; set; }
    public int TotalHours { get; set; }
}