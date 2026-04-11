using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace hw1
{
    public partial class MainForm : Form
    {
        //labels
        private Label lblStaff;
        private Label lblDay;
        private Label lblShiftType;
        private Label lblTitle;

        //dropdown-uri
        private ComboBox cmbStaff;
        private ComboBox cmbDay;
        private ComboBox cmbShiftType;

        //butoane
        private Button btnAdd;
        private Button btnDelete;
        private Button btnClear;

        //alte controale
        private CheckBox chkOvertime;
        private DataGridView dgvShifts;

        //coloane tabela
        private DataGridViewTextBoxColumn col1;
        private DataGridViewTextBoxColumn col2;
        private DataGridViewTextBoxColumn col3;
        private DataGridViewTextBoxColumn col4;

        //variabile
        private string currentUser;
        private string currentRole;
        private bool isInitializing;
        private List<Shift> shifts;

        public MainForm(string username, string role)
        {
            //salvam datele user-ului curent
            currentUser = username;
            currentRole = role;
            shifts = new List<Shift>();
            isInitializing = true;
            InitializeComponent();
            SetupRolePermissions();
            LoadShifts();
            isInitializing = false;
        }

        private void InitializeComponent()
        {
            //cream controalele
            lblTitle = new Label();
            lblStaff = new Label();
            lblDay = new Label();
            lblShiftType = new Label();
            cmbStaff = new ComboBox();
            cmbDay = new ComboBox();
            cmbShiftType = new ComboBox();
            chkOvertime = new CheckBox();
            btnAdd = new Button();
            btnDelete = new Button();
            btnClear = new Button();
            dgvShifts = new DataGridView();
            col1 = new DataGridViewTextBoxColumn();
            col2 = new DataGridViewTextBoxColumn();
            col3 = new DataGridViewTextBoxColumn();
            col4 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgvShifts).BeginInit();
            SuspendLayout();

            //titlu
            lblTitle.Font = new Font("Arial", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(225, 19);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(452, 73);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Shift Management";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            //eticheta staff
            lblStaff.Font = new Font("Arial", 10F);
            lblStaff.Location = new Point(127, 92);
            lblStaff.Name = "lblStaff";
            lblStaff.Size = new Size(133, 40);
            lblStaff.TabIndex = 2;
            lblStaff.Text = "Staff Member:";

            //eticheta zi
            lblDay.Font = new Font("Arial", 10F);
            lblDay.Location = new Point(290, 92);
            lblDay.Name = "lblDay";
            lblDay.Size = new Size(93, 40);
            lblDay.TabIndex = 4;
            lblDay.Text = "Day:";

            //eticheta tip tura
            lblShiftType.Font = new Font("Arial", 10F);
            lblShiftType.Location = new Point(457, 92);
            lblShiftType.Name = "lblShiftType";
            lblShiftType.Size = new Size(113, 40);
            lblShiftType.TabIndex = 6;
            lblShiftType.Text = "Shift Type:";

            //dropdown staff
            cmbStaff.Font = new Font("Arial", 11F);
            cmbStaff.Items.AddRange(new object[] { "waiter1", "waiter2", "chef1", "chef2" });
            cmbStaff.Location = new Point(127, 141);
            cmbStaff.Name = "cmbStaff";
            cmbStaff.Size = new Size(153, 41);
            cmbStaff.TabIndex = 3;

            //dropdown zile
            cmbDay.Font = new Font("Arial", 11F);
            cmbDay.Items.AddRange(new object[] { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" });
            cmbDay.Location = new Point(290, 141);
            cmbDay.Name = "cmbDay";
            cmbDay.Size = new Size(150, 41);
            cmbDay.TabIndex = 5;
            cmbDay.SelectedIndexChanged += CmbDay_SelectedIndexChanged;

            //dropdown tip tura
            cmbShiftType.Font = new Font("Arial", 11F);
            cmbShiftType.Items.AddRange(new object[] { "morning", "evening", "night" });
            cmbShiftType.Location = new Point(446, 141);
            cmbShiftType.Name = "cmbShiftType";
            cmbShiftType.Size = new Size(154, 41);
            cmbShiftType.TabIndex = 7;

            //checkbox overtime
            chkOvertime.Font = new Font("Arial", 11F);
            chkOvertime.Location = new Point(697, 136);
            chkOvertime.Name = "chkOvertime";
            chkOvertime.Size = new Size(179, 51);
            chkOvertime.TabIndex = 8;
            chkOvertime.Text = "Overtime";
            chkOvertime.CheckedChanged += ChkOvertime_CheckedChanged;

            //buton add
            btnAdd.BackColor = Color.FromArgb(0, 150, 80);
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Arial", 11F, FontStyle.Bold);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(127, 194);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(153, 48);
            btnAdd.TabIndex = 9;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += BtnAdd_Click;

            //buton delete
            btnDelete.BackColor = Color.FromArgb(200, 50, 50);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Arial", 11F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(287, 194);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(153, 48);
            btnDelete.TabIndex = 10;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += BtnDelete_Click;

            //buton clear all
            btnClear.BackColor = Color.Gray;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Font = new Font("Arial", 11F, FontStyle.Bold);
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(447, 194);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(153, 48);
            btnClear.TabIndex = 11;
            btnClear.Text = "Clear All";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += BtnClear_Click;

            //tabela cu ture
            dgvShifts.AllowUserToAddRows = false;
            dgvShifts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvShifts.BackgroundColor = Color.White;
            dgvShifts.ColumnHeadersHeight = 35;
            dgvShifts.Columns.AddRange(new DataGridViewColumn[] { col1, col2, col3, col4 });
            dgvShifts.Font = new Font("Arial", 11F);
            dgvShifts.Location = new Point(117, 276);
            dgvShifts.MultiSelect = false;
            dgvShifts.Name = "dgvShifts";
            dgvShifts.ReadOnly = true;
            dgvShifts.RowHeadersVisible = false;
            dgvShifts.RowHeadersWidth = 82;
            dgvShifts.RowTemplate.Height = 30;
            dgvShifts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvShifts.Size = new Size(700, 400);
            dgvShifts.TabIndex = 12;
            dgvShifts.SelectionChanged += DgvShifts_SelectionChanged;

            //coloane tabela
            col1.HeaderText = "Staff";
            col1.MinimumWidth = 10;
            col1.Name = "col1";
            col1.ReadOnly = true;

            col2.HeaderText = "Day";
            col2.MinimumWidth = 10;
            col2.Name = "col2";
            col2.ReadOnly = true;

            col3.HeaderText = "Shift Type";
            col3.MinimumWidth = 10;
            col3.Name = "col3";
            col3.ReadOnly = true;

            col4.HeaderText = "Overtime";
            col4.MinimumWidth = 10;
            col4.Name = "col4";
            col4.ReadOnly = true;

            //setari forma
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(240, 240, 245);
            ClientSize = new Size(970, 739);
            Controls.Add(lblTitle);
            Controls.Add(lblStaff);
            Controls.Add(cmbStaff);
            Controls.Add(lblDay);
            Controls.Add(cmbDay);
            Controls.Add(lblShiftType);
            Controls.Add(cmbShiftType);
            Controls.Add(chkOvertime);
            Controls.Add(btnAdd);
            Controls.Add(btnDelete);
            Controls.Add(btnClear);
            Controls.Add(dgvShifts);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Staff Management - Shift Scheduling";
            ((System.ComponentModel.ISupportInitialize)dgvShifts).EndInit();
            ResumeLayout(false);
        }

        //seteaza ce poate face fiecare user in functie de rol
        private void SetupRolePermissions()
        {
            if (currentRole == "Admin")
            {
                //admin are acces complet
                cmbStaff.Enabled = true;
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
                btnClear.Enabled = true;
                cmbStaff.SelectedIndex = 0;
            }
            else
            {
                //pentru waiter/chef, blocam dropdown-ul pe user-ul curent
                int index = 0;
                for (int i = 0; i < cmbStaff.Items.Count; i++)
                {
                    if (cmbStaff.Items[i].ToString() == currentUser)
                    {
                        index = i;
                        break;
                    }
                }

                cmbStaff.SelectedIndex = index;
                cmbStaff.Enabled = false;
                btnAdd.Enabled = true;
                btnDelete.Enabled = false;
                btnClear.Enabled = false;
            }
        }

        //incarca turele din fisier
        private void LoadShifts()
        {
            shifts.Clear();

            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shifts.txt");

                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string[] parts = line.Split(',');

                        if (parts.Length >= 3)
                        {
                            Shift s = new Shift();
                            s.Staff = parts[0];
                            s.Day = parts[1];
                            s.ShiftType = parts[2];

                            //parsam overtime
                            if (parts.Length > 3)
                            {
                                if (parts[3] == "yes" || parts[3] == "true" || parts[3] == "True")
                                {
                                    s.Overtime = true;
                                }
                                else
                                {
                                    s.Overtime = false;
                                }
                            }
                            else
                            {
                                s.Overtime = false;
                            }

                            shifts.Add(s);
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

        //salveaza turele in fisier
        private void SaveShifts()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shifts.txt");
                List<string> lines = new List<string>();

                for (int i = 0; i < shifts.Count; i++)
                {
                    Shift s = shifts[i];
                    string overtimeStr = "no";

                    if (s.Overtime == true)
                    {
                        overtimeStr = "yes";
                    }

                    string line = s.Staff + "," + s.Day + "," + s.ShiftType + "," + overtimeStr;
                    lines.Add(line);
                }

                File.WriteAllLines(path, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving shifts: " + ex.Message);
            }
        }

        //actualizeaza tabela cu date
        private void RefreshGrid()
        {
            dgvShifts.Rows.Clear();
            dgvShifts.ClearSelection();

            for (int i = 0; i < shifts.Count; i++)
            {
                Shift s = shifts[i];
                string overtimeText = "No";

                if (s.Overtime == true)
                {
                    overtimeText = "Yes";
                }

                dgvShifts.Rows.Add(s.Staff, s.Day, s.ShiftType, overtimeText);
            }
        }

        //buton add
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            //verificam permisiunile
            if (currentRole != "Admin")
            {
                string selectedStaff = cmbStaff.SelectedItem.ToString();
                if (selectedStaff != currentUser)
                {
                    MessageBox.Show("You can only add shifts for yourself!", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            //luam datele din controale
            string staff = cmbStaff.SelectedItem.ToString();
            string day = cmbDay.SelectedItem.ToString();
            string shiftType = cmbShiftType.SelectedItem.ToString();
            bool overtime = chkOvertime.Checked;

            //cautam daca exista deja o tura pentru aceeasi persoana si zi
            Shift existing = null;
            for (int i = 0; i < shifts.Count; i++)
            {
                if (shifts[i].Staff == staff && shifts[i].Day == day)
                {
                    existing = shifts[i];
                    break;
                }
            }

            //daca exista, o stergem ca sa o inlocuim
            if (existing != null)
            {
                shifts.Remove(existing);
            }

            //adaugam tura noua
            Shift newShift = new Shift();
            newShift.Staff = staff;
            newShift.Day = day;
            newShift.ShiftType = shiftType;
            newShift.Overtime = overtime;

            shifts.Add(newShift);
            SaveShifts();
            RefreshGrid();
            MessageBox.Show("Shift added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //buton delete
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvShifts.SelectedRows.Count > 0)
            {
                int index = dgvShifts.SelectedRows[0].Index;

                if (index < shifts.Count)
                {
                    string staff = shifts[index].Staff;

                    //verificam permisiunile
                    if (currentRole != "Admin")
                    {
                        if (staff != currentUser)
                        {
                            MessageBox.Show("You can only delete your own shifts!", "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    shifts.RemoveAt(index);
                    SaveShifts();
                    RefreshGrid();
                    MessageBox.Show("Shift deleted!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a shift to delete from the table.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //buton clear all
        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (currentRole == "Admin")
            {
                DialogResult result = MessageBox.Show("Are you sure you want to clear all shifts?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    shifts.Clear();
                    SaveShifts();
                    RefreshGrid();
                    MessageBox.Show("All shifts cleared!", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        //cand selectam un rand in tabela, populam dropdown-urile
        private void DgvShifts_SelectionChanged(object sender, EventArgs e)
        {
            //evitam bug-uri la incarcare
            if (isInitializing)
            {
                return;
            }

            //pentru non-admin, nu schimbam dropdown-ul de staff
            if (currentRole != "Admin")
            {
                return;
            }

            if (dgvShifts.SelectedRows.Count > 0)
            {
                int index = dgvShifts.SelectedRows[0].Index;

                if (index < shifts.Count)
                {
                    Shift shift = shifts[index];

                    //setam dropdown-ul de staff
                    string staffToSet = shift.Staff;
                    for (int i = 0; i < cmbStaff.Items.Count; i++)
                    {
                        if (cmbStaff.Items[i].ToString() == staffToSet)
                        {
                            cmbStaff.SelectedIndex = i;
                            break;
                        }
                    }

                    //setam dropdown-ul de zi
                    string dayToSet = shift.Day;
                    for (int i = 0; i < cmbDay.Items.Count; i++)
                    {
                        if (cmbDay.Items[i].ToString() == dayToSet)
                        {
                            cmbDay.SelectedIndex = i;
                            break;
                        }
                    }

                    //setam dropdown-ul de tip tura
                    string shiftTypeToSet = shift.ShiftType;
                    for (int i = 0; i < cmbShiftType.Items.Count; i++)
                    {
                        if (cmbShiftType.Items[i].ToString() == shiftTypeToSet)
                        {
                            cmbShiftType.SelectedIndex = i;
                            break;
                        }
                    }

                    chkOvertime.Checked = shift.Overtime;
                }
            }
        }

        //cand se schimba ziua selectata
        private void CmbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            //doar un eveniment, nu facem nimic special aici
        }

        //cand se schimba checkbox-ul overtime
        private void ChkOvertime_CheckedChanged(object sender, EventArgs e)
        {
            //doar un eveniment, nu facem nimic special aici
        }
    }

    //clasa pentru o tura
    public class Shift
    {
        public string Staff;
        public string Day;
        public string ShiftType;
        public bool Overtime;
    }
}
