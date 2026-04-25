using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Invetory
{
    public partial class Form1 : Form
    {
        BindingList<Product> inventory = new BindingList<Product>();
        public Form1()
        {
            InitializeComponent();
            
            dataGridView1.DataSource = inventory;
        }

        //ADD ceva [e aici
        private void button_Add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Product.Text) ||
                comboBox_Category.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(textBox_Unit.Text) ||
                numericUpDown_Quantity.Value <= 0 ||
                numericUpDown_Min_Stock.Value <=0)
            {
                MessageBox.Show("Please fill in all fields before adding!",
                                "Missing Information",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return; 
            }

            var newProd = new Product
            {
                ID = inventory.Count + 1,
                ProductName = textBox_Product.Text,
                Category = comboBox_Category.Text,
                Quantity = (int)numericUpDown_Quantity.Value,
                Unit = textBox_Unit.Text, 
                Min_Stock = (int)numericUpDown_Min_Stock.Value
            };

            inventory.Add(newProd);
            ClearInputs();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //DELETE
        private void button_Delete_Click(object sender, EventArgs e)
        {
            // ce metoda.. mai bine mergea cu for
            var itemToRemove = inventory.FirstOrDefault(p => p.ProductName == textBox_Product.Text);

            if (itemToRemove != null)
            {
                inventory.Remove(itemToRemove);
            }
            else
            {
                MessageBox.Show("Product not found!");
            }
        }

        private void ClearInputs()
        {
            textBox_Product.Clear();       
            comboBox_Category.SelectedIndex = -1; 
            numericUpDown_Quantity.Value = 0;     
            textBox_Unit.Clear();          
            numericUpDown_Min_Stock.Value = 0;    
            textBox_Product.Focus();
        }

        //SELECT CATEGORY
        private void comboBox_Category_SelectedIndexChanged(object sender, EventArgs e)
        {
            string select_Cat=comboBox_Category.Text;

            if(string.IsNullOrEmpty(select_Cat) || select_Cat=="All")
            {
                dataGridView1.DataSource= inventory;
            }
            else
            {
                BindingList<Product> filteredResults = new BindingList<Product>();

                foreach (Product p in inventory)
                {
                    if (p.Category == select_Cat)
                    {
                        filteredResults.Add(p);
                    }
                }
                dataGridView1.DataSource = filteredResults;
            }
        }

        //EDIT
        private void button_Edit_Click(object sender, EventArgs e)
        {
            string Edit_Prod= textBox_Product.Text;

            if (string.IsNullOrWhiteSpace(Edit_Prod))
            {
                MessageBox.Show("Please enter the name of the product you wish to edit.");
                return;
            }

            bool found = false;
            foreach (Product p in inventory)
            {
                if (p.ProductName.Equals(Edit_Prod, StringComparison.OrdinalIgnoreCase))
                {
                    
                    if (!string.IsNullOrEmpty(comboBox_Category.Text))
                    {
                        p.Category = comboBox_Category.Text;
                    }

                    if (numericUpDown_Quantity.Value > 0)
                    {
                        p.Quantity = (int)numericUpDown_Quantity.Value;
                    }

                    if (!string.IsNullOrWhiteSpace(textBox_Unit.Text))
                    {
                        p.Unit = textBox_Unit.Text;
                    }

                    if (numericUpDown_Min_Stock.Value > 0)
                    {
                        p.Quantity = (int)numericUpDown_Min_Stock.Value;
                    }


                    found = true;
                    break;
                }
            }

            if (found)
            {
                inventory.ResetBindings();
                MessageBox.Show("Product updated successfully!");
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Product not found. To edit, the Name must match exactly.");
            }
        }

        private void numericUpDown_Min_Stock_ValueChanged(object sender, EventArgs e)
        {

        }
    }
    public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public int Min_Stock { get; set; }
    }
}
