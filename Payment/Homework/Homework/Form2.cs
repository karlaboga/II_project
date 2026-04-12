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
    public partial class Form2 : Form
    {
        private List<OrderItem> orderItems;
        private double totalAmount;
        private double discountPercent;
        public Form2(List<OrderItem> items, double total, double discount)
        {
            orderItems = items;
            totalAmount = total;
            discountPercent = discount;

            InitializeComponent();
            Cash_input.TextChanged += TxtCashGiven_TextChanged;
            ApplyTheme();
        }
        private void ApplyTheme()
        {
            this.BackColor = ColorTranslator.FromHtml("#FBEAF0");
            Confirm_Button.BackColor = ColorTranslator.FromHtml("#D4537E");
            Confirm_Button.ForeColor = Color.White;
            Back_Button.BackColor = ColorTranslator.FromHtml("#F4C0D1");
        }
        private void UpdateCashVisibility()
        {
            bool isCash = Choose_Cash_Method.Checked;
            Cash_given.Visible = isCash;
            Cash_input.Visible = isCash;
            Change.Visible = isCash;
            Change_Value.Visible = isCash;

            if (!isCash)
                Change_Value.Text = "-";
           

        }

        private void TxtCashGiven_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(Cash_input.Text, out double given))
            {
                double changeAmount = given - totalAmount;
                Change_Value.Text = changeAmount >= 0
                    ? $"${changeAmount:0.00}"
                    : "Not enough cash";
                Change_Value.ForeColor = changeAmount >= 0 ? Color.DarkGreen : Color.Red;
            }
            else
            {
                Change_Value.Text = "-";
                Change_Value.ForeColor = Color.Black;
            }
        }
            

        private void Form2_Load(object sender, EventArgs e)
        {
            Amount_Value.Text = $"${totalAmount:0.00}"; 
            UpdateCashVisibility();
        }

        private void Choose_Cash_Method_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCashVisibility();
        }

        private void Back_Button_Click(object sender, EventArgs e)
             => this.Close();

        private void Confirm_Button_Click(object sender, EventArgs e)
        {
            if (Choose_Cash_Method.Checked)
            {
                if (!double.TryParse(Cash_input.Text, out double given)
                    || given < totalAmount)
                {
                    MessageBox.Show("Please enter a valid cash amount that covers the total.",
                        "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double changeAmount = given - totalAmount;
                MessageBox.Show(
                    $"Payment confirmed!\n\nTotal: ${totalAmount:0.00}\n" +
                    $"Cash Given: ${given:0.00}\nChange: ${changeAmount:0.00}",
                    "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Card payment of ${totalAmount:0.00} confirmed!\nThank you!",
                    "Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.Close();
        }

        private void Title_Click(object sender, EventArgs e)
        {

        }
    }
}
