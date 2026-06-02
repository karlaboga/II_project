using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StaffManagement;

public partial class LoginWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    private Dictionary<string, (string password, string role)> users = new();
    private bool _passwordVisible = false;

    public string LoggedInUser { get; private set; } = "";
    public string LoggedInRole { get; private set; } = "";

    public LoginWindow()
    {
        InitializeComponent();
        LoadUsers();
        Loaded += (_, _) => txtUsername.Focus();
    }

    private void LoadUsers()
    {
        users.Clear();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Username, Password, Role FROM Users", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string username = reader["Username"]?.ToString() ?? "";
                string password = reader["Password"]?.ToString() ?? "";
                string role = reader["Role"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(username))
                    users[username] = (password, role);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database connection error: " + ex.Message);
        }
    }

    private void PasswordField_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            BtnLogin_Click(sender, e);
        }
    }

    private void BtnShowPassword_Click(object sender, RoutedEventArgs e)
    {
        _passwordVisible = !_passwordVisible;

        if (_passwordVisible)
        {
            txtPasswordVisible.Text = txtPassword.Password;
            txtPassword.Visibility = Visibility.Collapsed;
            txtPasswordVisible.Visibility = Visibility.Visible;
            txtPasswordVisible.Focus();
            txtPasswordVisible.CaretIndex = txtPasswordVisible.Text.Length;
        }
        else
        {
            txtPassword.Password = txtPasswordVisible.Text;
            txtPasswordVisible.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Focus();
        }
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUsername.Text.Trim();

        string password = _passwordVisible
            ? txtPasswordVisible.Text
            : txtPassword.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            lblError.Text = "Please enter username and password!";
            return;
        }

        if (users.TryGetValue(username, out var userData))
        {
            if (password == userData.password)
            {
                LoggedInUser = username;
                LoggedInRole = userData.role;
                DialogResult = true;
                Close();
            }
            else
            {
                lblError.Text = "Invalid username or password!";
                txtPassword.Password = "";
                txtPasswordVisible.Text = "";
            }
        }
        else
        {
            lblError.Text = "Invalid username or password!";
            txtPassword.Password = "";
            txtPasswordVisible.Text = "";
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}