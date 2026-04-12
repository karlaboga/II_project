using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Homework
{
   
    

    public partial class Form1 : Form
    {
        private List<OrderItem> orderItems = new List<OrderItem>
        {
            new OrderItem { Name = "Cappucino", Quantity = 3, Price = 12.50 },
            new OrderItem { Name = "Tiramisu",  Quantity = 1, Price = 7.00  },
            new OrderItem { Name = "IceCream", Quantity = 3, Price = 8.50  }
        };

        private double discountPercent = 0;
        public Form1()
        {
            InitializeComponent();
            SetupGrid();
            LoadGrid();
            RefreshTotals();
        }
        private void SetupGrid()
        {
            Order_Summary.Columns.Clear();
            Order_Summary.AllowUserToAddRows = false;
            Order_Summary.RowHeadersVisible = false;
            Order_Summary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Order_Summary.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Item", ReadOnly = true });
            Order_Summary.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQty", HeaderText = "Qty", FillWeight = 40 });
            Order_Summary.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPrice", HeaderText = "Unit Price", ReadOnly = true, FillWeight = 60 });
            Order_Summary.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTotal", HeaderText = "Total", ReadOnly = true, FillWeight = 60 });

            Order_Summary.CellEndEdit += DgvOrder_CellEndEdit;
        }
        private void LoadGrid()
        {
            Order_Summary.Rows.Clear();
            foreach (var item in orderItems)
                Order_Summary.Rows.Add(item.Name, item.Quantity,
                                  $"{item.Price:0.00}", $"{item.Total:0.00}");
        }
        private void RefreshTotals()
        {
            double subtotalVal = 0;
            foreach (var item in orderItems) subtotalVal += item.Total;

            double discountAmount = subtotalVal * discountPercent / 100.0;
            double totalVal = subtotalVal - discountAmount;

            Subtotal_value.Text = $"{subtotalVal:0.00}";
            Discount_Value.Text = discountPercent > 0
                ? $"-{discountAmount:0.00} ({discountPercent}%)"
                : "0.00";
            Total_Value.Text = $"${totalVal:0.00}";
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = ColorTranslator.FromHtml("#FBEAF0");
            Pay_Button.BackColor = ColorTranslator.FromHtml("#D4537E");
            Discount_Button.BackColor = ColorTranslator.FromHtml("#993556");
            Edit_Qty_Button.BackColor = ColorTranslator.FromHtml("#F4C0D1");
            Restaurant_Name.ForeColor = ColorTranslator.FromHtml("#993556");
        }

        private void Edit_Qty_Button_Click(object sender, EventArgs e)
        {
            bool isEditing = Edit_Qty_Button.Text == "Edit Order";

            Order_Summary.ReadOnly = !isEditing;
            Order_Summary.BackgroundColor = isEditing ? Color.LightYellow : Color.White;
            Edit_Qty_Button.Text = isEditing ? "Done Editing" : "Edit Order";

            if (isEditing)
            {
                Order_Summary.Columns["colName"].ReadOnly = true;
                Order_Summary.Columns["colPrice"].ReadOnly = true;
                Order_Summary.Columns["colTotal"].ReadOnly = true;
                Order_Summary.Columns["colQty"].ReadOnly = false;
            }
        }
        private void DgvOrder_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != Order_Summary.Columns["colQty"].Index) return;

            var cell = Order_Summary.Rows[e.RowIndex].Cells["colQty"];

            if (int.TryParse(cell.Value?.ToString(), out int qty) && qty > 0)
            {
                orderItems[e.RowIndex].Quantity = qty;
                Order_Summary.Rows[e.RowIndex].Cells["colTotal"].Value
                    = $"{orderItems[e.RowIndex].Total:0.00}";
            }
            else
            {
                cell.Value = orderItems[e.RowIndex].Quantity;
            }

            RefreshTotals();
        }
        private void Order_Summary_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Discount_Button_Click(object sender, EventArgs e)
        {
            Form popup = new Form
            {
                Text = "Add Discount",
                Size = new Size(320, 210),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
            var lbl = new Label
            {
                Text = "Enter discount percentage:",
                Location = new Point(15, 20),
                Size = new Size(270, 20)
            };

            var txtDisc = new TextBox
            {
                Location = new Point(15, 50),
                Size = new Size(100, 25),
                Text = discountPercent > 0 ? discountPercent.ToString() : ""
            };

            var lblCalc = new Label
            {
                Text = "Calculated discount: 0.00",
                Location = new Point(15, 85),
                Size = new Size(270, 20),
                ForeColor = Color.DarkGreen
            };

            txtDisc.TextChanged += (s, ev) =>
            {
                if (double.TryParse(txtDisc.Text, out double pct))
                {
                    double sub = 0;
                    foreach (var item in orderItems) sub += item.Total;
                    lblCalc.Text = $"Calculated discount: {sub * pct / 100.0:0.00}";
                }
                else
                    lblCalc.Text = "Calculated discount: 0.00";
            };

            var btnApply = new Button
            {
                Text = "Apply",
                Location = new Point(15, 120),
                Size = new Size(90, 32),
                DialogResult = DialogResult.OK
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(120, 120),
                Size = new Size(90, 32),
                DialogResult = DialogResult.Cancel
            };

            popup.Controls.AddRange(new Control[] { lbl, txtDisc, lblCalc, btnApply, btnCancel });
            popup.AcceptButton = btnApply;
            popup.CancelButton = btnCancel;

            if (popup.ShowDialog(this) == DialogResult.OK)
            {
                if (double.TryParse(txtDisc.Text, out double pct) && pct >= 0 && pct <= 100)
                {
                    discountPercent = pct;
                    RefreshTotals();
                    MessageBox.Show("Discount of {pct}% applied!", "Done",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please enter a value between 0 and 100.", "Invalid",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Pay_Button_Click(object sender, EventArgs e)
        {
            double subtotalVal = 0;
            foreach (var item in orderItems) subtotalVal += item.Total;
            double totalVal = subtotalVal - (subtotalVal * discountPercent / 100.0);

            Form2 payForm = new Form2 (orderItems, totalVal, discountPercent);
            payForm.ShowDialog(this);
        }

        private void Subtotal_Click(object sender, EventArgs e)
        {

        }
    }
    public class OrderItem
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total => Quantity * Price;
    };

}
