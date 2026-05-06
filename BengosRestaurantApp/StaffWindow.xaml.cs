using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BengosRestaurantApp
{
    public partial class StaffWindow : Window
    {
        private ObservableCollection<Shift> shifts;
        private string currentUser;
        private string currentRole;
        private bool isInitializing = true;

        public StaffWindow(string username, string role)
        {
            InitializeComponent();
            currentUser = username;
            currentRole = role;
            shifts = new ObservableCollection<Shift>();
            DgShifts.ItemsSource = shifts;
            SetupRolePermissions();
            LoadShifts();
            isInitializing = false;
        }

        private void SetupRolePermissions()
        {
            if (currentRole == "Admin")
            {
                CmbStaff.IsEnabled = true;
                BtnAdd.IsEnabled = true;
                BtnDelete.IsEnabled = true;
                BtnClear.IsEnabled = true;
                CmbStaff.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < CmbStaff.Items.Count; i++)
                {
                    if (((ComboBoxItem)CmbStaff.Items[i]).Content.ToString() == currentUser)
                    {
                        CmbStaff.SelectedIndex = i;
                        break;
                    }
                }
                CmbStaff.IsEnabled = false;
                BtnAdd.IsEnabled = true;
                BtnDelete.IsEnabled = false;
                BtnClear.IsEnabled = false;
            }
        }

        private void LoadShifts()
        {
            shifts.Clear();

            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "shifts.txt");

                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] parts = lines[i].Split(',');

                        if (parts.Length >= 3)
                        {
                            var shift = new Shift
                            {
                                Staff = parts[0],
                                Day = parts[1],
                                ShiftType = parts[2],
                                Overtime = parts.Length > 3 && (parts[3] == "yes" || parts[3] == "true")
                            };
                            shifts.Add(shift);
                        }
                    }
                }

                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading shifts: " + ex.Message);
            }
        }

        private void SaveShifts()
        {
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "shifts.txt");
                var lines = new System.Collections.Generic.List<string>();

                for (int i = 0; i < shifts.Count; i++)
                {
                    string overtimeStr = shifts[i].Overtime ? "yes" : "no";
                    lines.Add($"{shifts[i].Staff},{shifts[i].Day},{shifts[i].ShiftType},{overtimeStr}");
                }

                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving shifts: " + ex.Message);
            }
        }

        private void RefreshGrid()
        {
            DgShifts.Items.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (currentRole != "Admin" && ((ComboBoxItem)CmbStaff.SelectedItem).Content.ToString() != currentUser)
            {
                MessageBox.Show("You can only add shifts for yourself!", "Permission Denied",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string staff = ((ComboBoxItem)CmbStaff.SelectedItem).Content.ToString();
            string day = ((ComboBoxItem)CmbDay.SelectedItem)?.Content.ToString();
            string shiftType = ((ComboBoxItem)CmbShiftType.SelectedItem)?.Content.ToString();
            bool overtime = ChkOvertime.IsChecked ?? false;

            if (string.IsNullOrEmpty(day) || string.IsNullOrEmpty(shiftType))
            {
                MessageBox.Show("Please select day and shift type!");
                return;
            }

            var existing = shifts.FirstOrDefault(s => s.Staff == staff && s.Day == day);
            if (existing != null)
            {
                shifts.Remove(existing);
            }

            shifts.Add(new Shift { Staff = staff, Day = day, ShiftType = shiftType, Overtime = overtime });
            SaveShifts();
            RefreshGrid();
            MessageBox.Show("Shift added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgShifts.SelectedItem is Shift selectedShift)
            {
                if (currentRole != "Admin" && selectedShift.Staff != currentUser)
                {
                    MessageBox.Show("You can only delete your own shifts!", "Permission Denied",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                shifts.Remove(selectedShift);
                SaveShifts();
                RefreshGrid();
                MessageBox.Show("Shift deleted!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a shift to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            if (currentRole == "Admin")
            {
                var result = MessageBox.Show("Are you sure you want to clear all shifts?", "Confirm Clear",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    shifts.Clear();
                    SaveShifts();
                    RefreshGrid();
                    MessageBox.Show("All shifts cleared!", "Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DgShifts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitializing || currentRole != "Admin")
                return;

            if (DgShifts.SelectedItem is Shift shift)
            {
                for (int i = 0; i < CmbStaff.Items.Count; i++)
                {
                    if (((ComboBoxItem)CmbStaff.Items[i]).Content.ToString() == shift.Staff)
                    {
                        CmbStaff.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < CmbDay.Items.Count; i++)
                {
                    if (((ComboBoxItem)CmbDay.Items[i]).Content.ToString() == shift.Day)
                    {
                        CmbDay.SelectedIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < CmbShiftType.Items.Count; i++)
                {
                    if (((ComboBoxItem)CmbShiftType.Items[i]).Content.ToString() == shift.ShiftType)
                    {
                        CmbShiftType.SelectedIndex = i;
                        break;
                    }
                }

                ChkOvertime.IsChecked = shift.Overtime;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class Shift
    {
        public string Staff { get; set; }
        public string Day { get; set; }
        public string ShiftType { get; set; }
        public bool Overtime { get; set; }
        public string OvertimeText => Overtime ? "Yes" : "No";
    }
}
