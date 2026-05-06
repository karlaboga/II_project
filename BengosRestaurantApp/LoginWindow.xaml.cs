using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BengosRestaurantApp
{
    public partial class LoginWindow : Window
    {
        private Dictionary<string, (string password, string role)> users;

        public string Username { get; private set; }
        public string Role { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            users = new Dictionary<string, (string, string)>();

            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "users.txt");

                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] parts = lines[i].Split(',');

                        if (parts.Length >= 3)
                        {
                            users[parts[0]] = (parts[1], parts[2]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                LblError.Text = "Please enter username and password!";
                return;
            }

            if (users.ContainsKey(username))
            {
                var userData = users[username];

                if (password == userData.password)
                {
                    Username = username;
                    Role = userData.role;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    LblError.Text = "Invalid username or password!";
                    TxtPassword.Password = "";
                }
            }
            else
            {
                LblError.Text = "Invalid username or password!";
                TxtPassword.Password = "";
            }
        }
    }
}
