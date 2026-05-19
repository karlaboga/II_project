using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace StaffManagement;
public class StaffViewModel
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public ObservableCollection<Shift> Shifts { get; } = new();
    public ObservableCollection<string> StaffList { get; } = new();
    public Dictionary<string, int> UserMap { get; } = new();
    public List<string> DayList { get; } = new()
    {
        "monday", "tuesday", "wednesday", "thursday",
        "friday", "saturday", "sunday"
    };
    public List<string> ShiftTypeList { get; } = new()
    {
        "morning", "evening", "night"
    };
    private string _currentUser;
    private string _currentRole;
    private string _selectedStaff = "";
    public string SelectedStaff
    {
        get => _selectedStaff;
        set => _selectedStaff = value ?? "";
    }
    private string _selectedDay = "";
    public string SelectedDay
    {
        get => _selectedDay;
        set => _selectedDay = value ?? "";
    }
    private string _selectedShiftType = "";
    public string SelectedShiftType
    {
        get => _selectedShiftType;
        set => _selectedShiftType = value ?? "";
    }
    private bool _overtime;
    public bool Overtime
    {
        get => _overtime;
        set => _overtime = value;
    }
    private Shift? _selectedShift;
    public Shift? SelectedShift
    {
        get => _selectedShift;
        set
        {
            _selectedShift = value;
            if (value != null && _currentRole == "Admin")
            {
                SelectedStaff = value.Staff;
                SelectedDay = value.Day;
                SelectedShiftType = value.ShiftType;
                Overtime = value.Overtime;
            }
        }
    }
    public bool IsAdmin => _currentRole == "Admin";
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearAllCommand { get; }
    public StaffViewModel(string username, string role)
    {
        _currentUser = username;
        _currentRole = role;
        AddCommand = new RelayCommand(AddShift);
        DeleteCommand = new RelayCommand(DeleteShift, () => SelectedShift != null);
        ClearAllCommand = new RelayCommand(ClearAllShifts, () => _currentRole == "Admin");
        LoadStaffList();
        LoadShifts();
    }
    private void LoadStaffList()
    {
        StaffList.Clear();
        UserMap.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Username FROM Users ORDER BY Username", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string username = reader["Username"]?.ToString() ?? "";
                int id = Convert.ToInt32(reader["Id"]);
                StaffList.Add(username);
                UserMap[username] = id;
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error loading staff: " + ex.Message);
        }
    }
    public void LoadShifts()
    {
        Shifts.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT s.Id, u.Username, s.Day, s.ShiftType, s.Overtime 
                  FROM Shifts s JOIN Users u ON s.StaffId = u.Id 
                  ORDER BY 
                    CASE s.Day
                        WHEN 'monday' THEN 1 WHEN 'tuesday' THEN 2
                        WHEN 'wednesday' THEN 3 WHEN 'thursday' THEN 4
                        WHEN 'friday' THEN 5 WHEN 'saturday' THEN 6
                        WHEN 'sunday' THEN 7 ELSE 8
                    END, s.ShiftType", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Shifts.Add(new Shift
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Staff = reader["Username"]?.ToString() ?? "",
                    Day = reader["Day"]?.ToString() ?? "",
                    ShiftType = reader["ShiftType"]?.ToString() ?? "",
                    Overtime = Convert.ToBoolean(reader["Overtime"])
                });
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error loading shifts: " + ex.Message);
        }
    }
    private void AddShift()
    {
        if (string.IsNullOrWhiteSpace(SelectedStaff) ||
            string.IsNullOrWhiteSpace(SelectedDay) ||
            string.IsNullOrWhiteSpace(SelectedShiftType))
        {
            System.Windows.MessageBox.Show("Please select staff, day, and shift type.");
            return;
        }
        if (_currentRole != "Admin" && SelectedStaff != _currentUser)
        {
            System.Windows.MessageBox.Show("You can only add shifts for yourself!", "Permission Denied",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        if (!UserMap.TryGetValue(SelectedStaff, out int staffId))
        {
            System.Windows.MessageBox.Show("Staff member not found.");
            return;
        }
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var delCmd = new SqlCommand(
                "DELETE FROM Shifts WHERE StaffId = @sid AND Day = @day", conn);
            delCmd.Parameters.AddWithValue("@sid", staffId);
            delCmd.Parameters.AddWithValue("@day", SelectedDay);
            delCmd.ExecuteNonQuery();
            using var insCmd = new SqlCommand(
                "INSERT INTO Shifts (StaffId, Day, ShiftType, Overtime) VALUES (@sid, @day, @type, @ot)", conn);
            insCmd.Parameters.AddWithValue("@sid", staffId);
            insCmd.Parameters.AddWithValue("@day", SelectedDay);
            insCmd.Parameters.AddWithValue("@type", SelectedShiftType);
            insCmd.Parameters.AddWithValue("@ot", Overtime);
            insCmd.ExecuteNonQuery();
            LoadShifts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error adding shift: " + ex.Message);
        }
    }
    private void DeleteShift()
    {
        if (SelectedShift == null) return;
        if (_currentRole != "Admin" && SelectedShift.Staff != _currentUser)
        {
            System.Windows.MessageBox.Show("You can only delete your own shifts!", "Permission Denied",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        if (System.Windows.MessageBox.Show("Delete this shift?", "Confirm",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) !=
            System.Windows.MessageBoxResult.Yes)
            return;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Shifts WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", SelectedShift.Id);
            cmd.ExecuteNonQuery();
            LoadShifts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error deleting shift: " + ex.Message);
        }
    }
    private void ClearAllShifts()
    {
        if (System.Windows.MessageBox.Show("Clear all shifts?", "Confirm",
                System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) !=
            System.Windows.MessageBoxResult.Yes)
            return;
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM Shifts", conn);
            cmd.ExecuteNonQuery();
            LoadShifts();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error clearing shifts: " + ex.Message);
        }
    }
}