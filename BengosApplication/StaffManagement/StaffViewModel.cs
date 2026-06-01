using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace StaffManagement;
public class StaffViewModel : INotifyPropertyChanged
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public ObservableCollection<Shift> Shifts { get; } = new();
    public ObservableCollection<string> StaffList { get; } = new();
    public Dictionary<string, int> UserMap { get; } = new();
    
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
        set { _selectedStaff = value ?? ""; OnPropertyChanged(); }
    }
    private DateTime _selectedDate = DateTime.Today;
    public DateTime SelectedDate
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
                  ORDER BY s.Day DESC, s.ShiftType", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string dayVal = reader["Day"]?.ToString() ?? "";
                DateTime shiftDate;
                if (!DateTime.TryParse(dayVal, out shiftDate)) shiftDate = DateTime.Today;

                Shifts.Add(new Shift
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Staff = reader["Username"]?.ToString() ?? "",
                    Date = shiftDate,
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
            string dateStr = SelectedDate.ToString("yyyy-MM-dd");
            using var delCmd = new SqlCommand(
                "DELETE FROM Shifts WHERE StaffId = @sid AND Day = @day", conn);
            delCmd.Parameters.AddWithValue("@sid", staffId);
            delCmd.Parameters.AddWithValue("@day", dateStr);
            delCmd.ExecuteNonQuery();
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