using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace StaffManagement;
public class StaffViewModel : INotifyPropertyChanged
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public ObservableCollection<Shift> AdminShifts { get; } = new();
    public ObservableCollection<Shift> EmployeeShifts { get; } = new();
    public ObservableCollection<Shift> CookShifts { get; } = new();
    public ObservableCollection<string> StaffList { get; } = new();
    public Dictionary<string, int> UserMap { get; } = new();
    public Dictionary<string, string> UserRoleMap { get; } = new();
    
    public List<string> ShiftTypeList { get; } = new()
    {
        "", "morning", "evening", "night"
    };
    private string _currentUser;
    private string _currentRole;
    private string _selectedStaff = "";
    public string SelectedStaff
    {
        get => _selectedStaff;
        set { _selectedStaff = value ?? ""; OnPropertyChanged(); OnPropertyChanged(nameof(SelectedStaffRole)); }
    }
    public string SelectedStaffRole =>
        !string.IsNullOrEmpty(SelectedStaff) && UserRoleMap.TryGetValue(SelectedStaff, out var role) ? role : "";
    private DateTime? _selectedDate = DateTime.Today;
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set { _selectedDate = value; OnPropertyChanged(); }
    }
    private string _selectedShiftType = "";
    public string SelectedShiftType
    {
        get => _selectedShiftType;
        set { _selectedShiftType = value ?? ""; OnPropertyChanged(); }
    }
    private bool _overtime;
    public bool Overtime
    {
        get => _overtime;
        set { _overtime = value; OnPropertyChanged(); }
    }
    private Shift? _selectedShift;
    public Shift? SelectedShift
    {
        get => _selectedShift;
        set
        {
            _selectedShift = value;
            OnPropertyChanged();
            if (value != null && _currentRole == "Admin")
            {
                SelectedStaff = value.Staff;
                SelectedDate = value.Date;
                SelectedShiftType = value.ShiftType;
                Overtime = value.Overtime;
            }
        }
    }
    public bool IsAdmin => _currentRole == "Admin";
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearAllCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public StaffViewModel(string username, string role)
    {
        _currentUser = username;
        _currentRole = role;
        AddCommand = new RelayCommand(AddShift);
        DeleteCommand = new RelayCommand(DeleteShift, () => SelectedShift != null);
        ClearAllCommand = new RelayCommand(ClearAllShifts, () => _currentRole == "Admin");
        SearchCommand = new RelayCommand(SearchShifts);
        RefreshCommand = new RelayCommand(LoadShifts);
        LoadStaffList();
        LoadShifts();
    }
    private void LoadStaffList()
    {
        StaffList.Clear();
        UserMap.Clear();
        UserRoleMap.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, Username, Role FROM Users ORDER BY Username", conn);
            StaffList.Add("");
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string username = reader["Username"]?.ToString() ?? "";
                int id = Convert.ToInt32(reader["Id"]);
                string role = reader["Role"]?.ToString() ?? "";
                StaffList.Add(username);
                UserMap[username] = id;
                UserRoleMap[username] = role;
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error loading staff: " + ex.Message);
        }
    }
    public void LoadShifts()
    {
        AdminShifts.Clear();
        EmployeeShifts.Clear();
        CookShifts.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"SELECT s.Id, u.Username, u.Role, s.Day, s.ShiftType, s.Overtime 
                  FROM Shifts s JOIN Users u ON s.StaffId = u.Id 
                  ORDER BY s.Day DESC, 
                    CASE s.ShiftType 
                      WHEN 'morning' THEN 1 
                      WHEN 'evening' THEN 2 
                      WHEN 'night' THEN 3 
                      ELSE 4 
                    END", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string dayVal = reader["Day"]?.ToString() ?? "";
                DateTime shiftDate;
                if (!DateTime.TryParse(dayVal, out shiftDate)) shiftDate = DateTime.Today;

                var shift = new Shift
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Staff = reader["Username"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "",
                    Date = shiftDate,
                    ShiftType = reader["ShiftType"]?.ToString() ?? "",
                    Overtime = Convert.ToBoolean(reader["Overtime"])
                };

                switch (shift.Role)
                {
                    case "Admin":
                        AdminShifts.Add(shift);
                        break;
                    case "Employee":
                        EmployeeShifts.Add(shift);
                        break;
                    case "Cook":
                        CookShifts.Add(shift);
                        break;
                    default:
                        EmployeeShifts.Add(shift);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error loading shifts: " + ex.Message);
        }
    }
    public void SearchShifts()
    {
        AdminShifts.Clear();
        EmployeeShifts.Clear();
        CookShifts.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            var query = @"SELECT s.Id, u.Username, u.Role, s.Day, s.ShiftType, s.Overtime 
                          FROM Shifts s JOIN Users u ON s.StaffId = u.Id WHERE 1=1";
            var parameters = new List<(string name, object value)>();

            if (!string.IsNullOrWhiteSpace(SelectedStaff) && UserMap.TryGetValue(SelectedStaff, out int staffId))
            {
                query += " AND s.StaffId = @sid";
                parameters.Add(("@sid", staffId));
            }

            if (SelectedDate.HasValue)
            {
                query += " AND s.Day = @day";
                parameters.Add(("@day", SelectedDate.Value.ToString("yyyy-MM-dd")));
            }

            if (!string.IsNullOrWhiteSpace(SelectedShiftType))
            {
                query += " AND s.ShiftType = @type";
                parameters.Add(("@type", SelectedShiftType));
            }

            if (Overtime)
            {
                query += " AND s.Overtime = 1";
            }

            query += @" ORDER BY s.Day DESC, 
                        CASE s.ShiftType 
                          WHEN 'morning' THEN 1 
                          WHEN 'evening' THEN 2 
                          WHEN 'night' THEN 3 
                          ELSE 4 
                        END";

            using var cmd = new SqlCommand(query, conn);
            foreach (var (name, value) in parameters)
                cmd.Parameters.AddWithValue(name, value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string dayVal = reader["Day"]?.ToString() ?? "";
                DateTime shiftDate;
                if (!DateTime.TryParse(dayVal, out shiftDate)) shiftDate = DateTime.Today;

                var shift = new Shift
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Staff = reader["Username"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? "",
                    Date = shiftDate,
                    ShiftType = reader["ShiftType"]?.ToString() ?? "",
                    Overtime = Convert.ToBoolean(reader["Overtime"])
                };

                switch (shift.Role)
                {
                    case "Admin":
                        AdminShifts.Add(shift);
                        break;
                    case "Employee":
                        EmployeeShifts.Add(shift);
                        break;
                    case "Cook":
                        CookShifts.Add(shift);
                        break;
                    default:
                        EmployeeShifts.Add(shift);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("Error searching shifts: " + ex.Message);
        }
    }

    private void AddShift()
    {
        if (string.IsNullOrWhiteSpace(SelectedStaff) ||
            string.IsNullOrWhiteSpace(SelectedShiftType))
        {
            System.Windows.MessageBox.Show("Please select staff and shift type.");
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
            string dateStr = (SelectedDate ?? DateTime.Today).ToString("yyyy-MM-dd");

            // Check if this specific shift type already exists for this person on this day
            using var checkCmd = new SqlCommand(
                "SELECT COUNT(*) FROM Shifts WHERE StaffId = @sid AND Day = @day AND ShiftType = @type", conn);
            checkCmd.Parameters.AddWithValue("@sid", staffId);
            checkCmd.Parameters.AddWithValue("@day", dateStr);
            checkCmd.Parameters.AddWithValue("@type", SelectedShiftType);
            bool typeExists = (int)checkCmd.ExecuteScalar() > 0;

            if (!typeExists)
            {
                // Check if they already have 3 shifts
                using var countCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Shifts WHERE StaffId = @sid AND Day = @day", conn);
                countCmd.Parameters.AddWithValue("@sid", staffId);
                countCmd.Parameters.AddWithValue("@day", dateStr);
                int totalCount = (int)countCmd.ExecuteScalar();

                if (totalCount >= 3)
                {
                    System.Windows.MessageBox.Show("An employee can have at most 3 shifts per day.", "Limit Reached",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                // If it exists, we delete it first to "update" it with new Overtime value
                using var delCmd = new SqlCommand(
                    "DELETE FROM Shifts WHERE StaffId = @sid AND Day = @day AND ShiftType = @type", conn);
                delCmd.Parameters.AddWithValue("@sid", staffId);
                delCmd.Parameters.AddWithValue("@day", dateStr);
                delCmd.Parameters.AddWithValue("@type", SelectedShiftType);
                delCmd.ExecuteNonQuery();
            }

            using var insCmd = new SqlCommand(
                "INSERT INTO Shifts (StaffId, Day, ShiftType, Overtime) VALUES (@sid, @day, @type, @ot)", conn);
            insCmd.Parameters.AddWithValue("@sid", staffId);
            insCmd.Parameters.AddWithValue("@day", dateStr);
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