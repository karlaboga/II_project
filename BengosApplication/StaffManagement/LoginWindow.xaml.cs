using Microsoft.Data.SqlClient;
using System.Windows;
namespace StaffManagement;
public partial class LoginWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    private Dictionary<string, (string password, string role)> users = new();
    public string LoggedInUser { get; private set; } = "";
    public string LoggedInRole { get; private set; } = "";
    public LoginWindow()
    {
        InitializeComponent();
        LoadUsers();
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
    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Password;
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
            }
        }
        else
        {
            lblError.Text = "Invalid username or password!";
            txtPassword.Password = "";
        }
    }
}