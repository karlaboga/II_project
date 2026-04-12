namespace Homework
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Restaurant_Name = new System.Windows.Forms.Label();
            this.Order_Summary = new System.Windows.Forms.DataGridView();
            this.Edit_Qty_Button = new System.Windows.Forms.Button();
            this.Subtotal = new System.Windows.Forms.Label();
            this.Subtotal_value = new System.Windows.Forms.Label();
            this.Discount = new System.Windows.Forms.Label();
            this.Total = new System.Windows.Forms.Label();
            this.Discount_Value = new System.Windows.Forms.Label();
            this.Total_Value = new System.Windows.Forms.Label();
            this.Discount_Button = new System.Windows.Forms.Button();
            this.Pay_Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Order_Summary)).BeginInit();
            this.SuspendLayout();
            // 
            // Restaurant_Name
            // 
            this.Restaurant_Name.AutoSize = true;
            this.Restaurant_Name.Location = new System.Drawing.Point(181, 18);
            this.Restaurant_Name.Name = "Restaurant_Name";
            this.Restaurant_Name.Size = new System.Drawing.Size(47, 39);
            this.Restaurant_Name.TabIndex = 0;
            this.Restaurant_Name.Text = "BeNgOs\r\n\r\n\r\n";
            // 
            // Order_Summary
            // 
            this.Order_Summary.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(221)))), ((int)(((byte)(244)))));
            this.Order_Summary.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Order_Summary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Order_Summary.GridColor = System.Drawing.Color.RosyBrown;
            this.Order_Summary.Location = new System.Drawing.Point(88, 73);
            this.Order_Summary.Name = "Order_Summary";
            this.Order_Summary.Size = new System.Drawing.Size(240, 150);
            this.Order_Summary.TabIndex = 1;
            this.Order_Summary.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Order_Summary_CellContentClick);
            // 
            // Edit_Qty_Button
            // 
            this.Edit_Qty_Button.Location = new System.Drawing.Point(88, 242);
            this.Edit_Qty_Button.Name = "Edit_Qty_Button";
            this.Edit_Qty_Button.Size = new System.Drawing.Size(75, 23);
            this.Edit_Qty_Button.TabIndex = 2;
            this.Edit_Qty_Button.Text = "Edit Order";
            this.Edit_Qty_Button.UseVisualStyleBackColor = true;
            this.Edit_Qty_Button.Click += new System.EventHandler(this.Edit_Qty_Button_Click);
            // 
            // Subtotal
            // 
            this.Subtotal.AutoSize = true;
            this.Subtotal.Location = new System.Drawing.Point(207, 242);
            this.Subtotal.Name = "Subtotal";
            this.Subtotal.Size = new System.Drawing.Size(52, 13);
            this.Subtotal.TabIndex = 3;
            this.Subtotal.Text = "Subtotal: ";
            this.Subtotal.Click += new System.EventHandler(this.Subtotal_Click);
            // 
            // Subtotal_value
            // 
            this.Subtotal_value.AutoSize = true;
            this.Subtotal_value.Location = new System.Drawing.Point(281, 242);
            this.Subtotal_value.Name = "Subtotal_value";
            this.Subtotal_value.Size = new System.Drawing.Size(35, 13);
            this.Subtotal_value.TabIndex = 4;
            this.Subtotal_value.Text = "label1";
            // 
            // Discount
            // 
            this.Discount.AutoSize = true;
            this.Discount.Location = new System.Drawing.Point(207, 269);
            this.Discount.Name = "Discount";
            this.Discount.Size = new System.Drawing.Size(52, 13);
            this.Discount.TabIndex = 5;
            this.Discount.Text = "Discount:";
            this.Discount.Click += new System.EventHandler(this.label1_Click);
            // 
            // Total
            // 
            this.Total.AutoSize = true;
            this.Total.Location = new System.Drawing.Point(207, 297);
            this.Total.Name = "Total";
            this.Total.Size = new System.Drawing.Size(45, 13);
            this.Total.TabIndex = 6;
            this.Total.Text = "TOTAL:";
            this.Total.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Discount_Value
            // 
            this.Discount_Value.AutoSize = true;
            this.Discount_Value.Location = new System.Drawing.Point(281, 269);
            this.Discount_Value.Name = "Discount_Value";
            this.Discount_Value.Size = new System.Drawing.Size(35, 13);
            this.Discount_Value.TabIndex = 7;
            this.Discount_Value.Text = "label1";
            // 
            // Total_Value
            // 
            this.Total_Value.AutoSize = true;
            this.Total_Value.Location = new System.Drawing.Point(281, 297);
            this.Total_Value.Name = "Total_Value";
            this.Total_Value.Size = new System.Drawing.Size(35, 13);
            this.Total_Value.TabIndex = 8;
            this.Total_Value.Text = "label2";
            // 
            // Discount_Button
            // 
            this.Discount_Button.Location = new System.Drawing.Point(88, 287);
            this.Discount_Button.Name = "Discount_Button";
            this.Discount_Button.Size = new System.Drawing.Size(75, 23);
            this.Discount_Button.TabIndex = 9;
            this.Discount_Button.Text = "Discount %";
            this.Discount_Button.UseVisualStyleBackColor = true;
            this.Discount_Button.Click += new System.EventHandler(this.Discount_Button_Click);
            // 
            // Pay_Button
            // 
            this.Pay_Button.Location = new System.Drawing.Point(155, 360);
            this.Pay_Button.Name = "Pay_Button";
            this.Pay_Button.Size = new System.Drawing.Size(129, 23);
            this.Pay_Button.TabIndex = 10;
            this.Pay_Button.Text = "Pay Method";
            this.Pay_Button.UseVisualStyleBackColor = true;
            this.Pay_Button.Click += new System.EventHandler(this.Pay_Button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 450);
            this.Controls.Add(this.Pay_Button);
            this.Controls.Add(this.Discount_Button);
            this.Controls.Add(this.Total_Value);
            this.Controls.Add(this.Discount_Value);
            this.Controls.Add(this.Total);
            this.Controls.Add(this.Discount);
            this.Controls.Add(this.Subtotal_value);
            this.Controls.Add(this.Subtotal);
            this.Controls.Add(this.Edit_Qty_Button);
            this.Controls.Add(this.Order_Summary);
            this.Controls.Add(this.Restaurant_Name);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Order_Summary)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Restaurant_Name;
        private System.Windows.Forms.DataGridView Order_Summary;
        private System.Windows.Forms.Button Edit_Qty_Button;
        private System.Windows.Forms.Label Subtotal;
        private System.Windows.Forms.Label Subtotal_value;
        private System.Windows.Forms.Label Discount;
        private System.Windows.Forms.Label Total;
        private System.Windows.Forms.Label Discount_Value;
        private System.Windows.Forms.Label Total_Value;
        private System.Windows.Forms.Button Discount_Button;
        private System.Windows.Forms.Button Pay_Button;
    }
}

