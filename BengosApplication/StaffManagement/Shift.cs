using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace StaffManagement;
public class Shift : INotifyPropertyChanged
{
    private int _id;
    private string _staff = "";
    private string _role = "";
    private DateTime _date = DateTime.Today;
    private string _shiftType = "";
    private bool _overtime;
    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(); }
    }
    public string Staff
    {
        get => _staff;
        set { _staff = value; OnPropertyChanged(); }
    }
    public string Role
    {
        get => _role;
        set { _role = value; OnPropertyChanged(); }
    }
    public DateTime Date
    {
        get => _date;
        set { _date = value; OnPropertyChanged(); OnPropertyChanged(nameof(DateDisplay)); }
    }
    public string DateDisplay => Date.ToString("yyyy-MM-dd");
    public string ShiftType
    {
        get => _shiftType;
        set { _shiftType = value; OnPropertyChanged(); }
    }
    public bool Overtime
    {
        get => _overtime;
        set { _overtime = value; OnPropertyChanged(); OnPropertyChanged(nameof(OvertimeText)); }
    }
    public string OvertimeText => Overtime ? "Yes" : "No";
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}