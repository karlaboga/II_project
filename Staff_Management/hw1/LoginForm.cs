using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace hw1
{
    public partial class LoginForm : Form
    {
        //controale
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private Label lblError;

        //dictionar cu userii
        private Dictionary<string, Tuple<string, string>> users;

        //proprietati pentru a trimite datele la MainForm
        public string LoggedInUser { get; private set; } = "";
        public string LoggedInRole { get; private set; } = "";

        public LoginForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        //incarca userii din fisier
        private void LoadUsers()
        {
            users = new Dictionary<string, Tuple<string, string>>();

            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");

                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string[] parts = line.Split(',');

                        if (parts.Length >= 3)
                        {
                            //format: username,parola,rol
                            string username = parts[0];
                            string password = parts[1];
                            string role = parts[2];

                            users[username] = new Tuple<string, string>(password, role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void InitializeComponent()
        {
            //cream controalele
            lblTitle = new Label();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            lblError = new Label();

            SuspendLayout();

            //titlu
            lblTitle.Font = new Font("Arial", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(61, 59, 58);
            lblTitle.Location = new Point(96, 22);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(409, 77);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Staff Management System";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            //eticheta username
            lblUsername.Font = new Font("Arial", 10F);
            lblUsername.ForeColor = Color.FromArgb(70, 68, 67);
            lblUsername.Location = new Point(115, 114);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(150, 36);
            lblUsername.TabIndex = 1;
            lblUsername.Text = "Username:";

            //casuta username
            txtUsername.Font = new Font("Arial", 12F);
            txtUsername.BackColor = Color.FromArgb(255, 255, 255);
            txtUsername.ForeColor = Color.FromArgb(61, 59, 58);
            txtUsername.Location = new Point(115, 153);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(409, 44);
            txtUsername.TabIndex = 2;

            //eticheta parola
            lblPassword.Font = new Font("Arial", 10F);
            lblPassword.ForeColor = Color.FromArgb(70, 68, 67);
            lblPassword.Location = new Point(115, 224);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(150, 33);
            lblPassword.TabIndex = 3;
            lblPassword.Text = "Password:";

            //casuta parola
            txtPassword.Font = new Font("Arial", 12F);
            txtPassword.BackColor = Color.FromArgb(255, 255, 255);
            txtPassword.ForeColor = Color.FromArgb(61, 59, 58);
            txtPassword.Location = new Point(115, 260);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(409, 44);
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;

            //buton login
            btnLogin.BackColor = Color.FromArgb(62, 60, 59);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Arial", 12F, FontStyle.Bold);
            btnLogin.ForeColor = Color.FromArgb(255, 255, 255);
            btnLogin.Location = new Point(222, 335);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(189, 77);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += BtnLogin_Click;

            //eticheta eroare
            lblError.Font = new Font("Arial", 9F);
            lblError.ForeColor = Color.FromArgb(200, 50, 50);
            lblError.Location = new Point(115, 420);
            lblError.Name = "lblError";
            lblError.Size = new Size(409, 25);
            lblError.TabIndex = 6;
            lblError.Text = "";
            lblError.TextAlign = ContentAlignment.MiddleCenter;

            //setari forma
            BackColor = Color.FromArgb(238, 237, 237);
            ClientSize = new Size(649, 454);

            Controls.Add(lblTitle);
            Controls.Add(lblUsername);
            Controls.Add(txtUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(lblError);

            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Staff Login";

            ResumeLayout(false);
            PerformLayout();
        }

        //buton login
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            //verificam daca sunt goale
            if (username == "" || password == "")
            {
                lblError.Text = "Please enter username and password!";
                return;
            }

            //cautam user-ul in dictionar
            if (users.ContainsKey(username))
            {
                Tuple<string, string> userData = users[username];
                string storedPassword = userData.Item1;

                //verificam parola
                if (password == storedPassword)
                {
                    LoggedInUser = username;
                    LoggedInRole = userData.Item2;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblError.Text = "Invalid username or password!";
                    txtPassword.Text = "";
                }
            }
            else
            {
                lblError.Text = "Invalid username or password!";
                txtPassword.Text = "";
            }
        }
    }
}
