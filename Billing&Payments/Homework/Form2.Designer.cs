namespace Homework
{
    partial class Form2
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
            this.Title = new System.Windows.Forms.Label();
            this.Amount_label = new System.Windows.Forms.Label();
            this.Amount_Value = new System.Windows.Forms.Label();
            this.Choose_Cash_Method = new System.Windows.Forms.RadioButton();
            this.Choose_Card_Method = new System.Windows.Forms.RadioButton();
            this.Cash_given = new System.Windows.Forms.Label();
            this.Cash_input = new System.Windows.Forms.TextBox();
            this.Change = new System.Windows.Forms.Label();
            this.Change_Value = new System.Windows.Forms.Label();
            this.Confirm_Button = new System.Windows.Forms.Button();
            this.Back_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.AutoSize = true;
            this.Title.Location = new System.Drawing.Point(87, 54);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(133, 13);
            this.Title.TabIndex = 0;
            this.Title.Text = "Choose a payment method";
            this.Title.Click += new System.EventHandler(this.Title_Click);
            // 
            // Amount_label
            // 
            this.Amount_label.AutoSize = true;
            this.Amount_label.Location = new System.Drawing.Point(40, 113);
            this.Amount_label.Name = "Amount_label";
            this.Amount_label.Size = new System.Drawing.Size(72, 13);
            this.Amount_label.TabIndex = 1;
            this.Amount_label.Text = "Amount Due: ";
            // 
            // Amount_Value
            // 
            this.Amount_Value.AutoSize = true;
            this.Amount_Value.Location = new System.Drawing.Point(133, 113);
            this.Amount_Value.Name = "Amount_Value";
            this.Amount_Value.Size = new System.Drawing.Size(35, 13);
            this.Amount_Value.TabIndex = 2;
            this.Amount_Value.Text = "label2";
            // 
            // Choose_Cash_Method
            // 
            this.Choose_Cash_Method.AutoSize = true;
            this.Choose_Cash_Method.Location = new System.Drawing.Point(45, 159);
            this.Choose_Cash_Method.Name = "Choose_Cash_Method";
            this.Choose_Cash_Method.Size = new System.Drawing.Size(49, 17);
            this.Choose_Cash_Method.TabIndex = 3;
            this.Choose_Cash_Method.TabStop = true;
            this.Choose_Cash_Method.Text = "Cash\r\n";
            this.Choose_Cash_Method.UseVisualStyleBackColor = true;
            this.Choose_Cash_Method.CheckedChanged += new System.EventHandler(this.Choose_Cash_Method_CheckedChanged);
            // 
            // Choose_Card_Method
            // 
            this.Choose_Card_Method.AutoSize = true;
            this.Choose_Card_Method.Location = new System.Drawing.Point(173, 159);
            this.Choose_Card_Method.Name = "Choose_Card_Method";
            this.Choose_Card_Method.Size = new System.Drawing.Size(47, 17);
            this.Choose_Card_Method.TabIndex = 4;
            this.Choose_Card_Method.TabStop = true;
            this.Choose_Card_Method.Text = "Card";
            this.Choose_Card_Method.UseVisualStyleBackColor = true;
            // 
            // Cash_given
            // 
            this.Cash_given.AutoSize = true;
            this.Cash_given.Location = new System.Drawing.Point(42, 219);
            this.Cash_given.Name = "Cash_given";
            this.Cash_given.Size = new System.Drawing.Size(63, 13);
            this.Cash_given.TabIndex = 5;
            this.Cash_given.Text = "Cash given:";
            // 
            // Cash_input
            // 
            this.Cash_input.Location = new System.Drawing.Point(120, 216);
            this.Cash_input.Name = "Cash_input";
            this.Cash_input.Size = new System.Drawing.Size(100, 20);
            this.Cash_input.TabIndex = 6;
            // 
            // Change
            // 
            this.Change.AutoSize = true;
            this.Change.Location = new System.Drawing.Point(42, 270);
            this.Change.Name = "Change";
            this.Change.Size = new System.Drawing.Size(70, 13);
            this.Change.TabIndex = 7;
            this.Change.Text = "Change Due:";
            // 
            // Change_Value
            // 
            this.Change_Value.AutoSize = true;
            this.Change_Value.Location = new System.Drawing.Point(170, 270);
            this.Change_Value.Name = "Change_Value";
            this.Change_Value.Size = new System.Drawing.Size(35, 13);
            this.Change_Value.TabIndex = 8;
            this.Change_Value.Text = "label3";
            // 
            // Confirm_Button
            // 
            this.Confirm_Button.Location = new System.Drawing.Point(120, 315);
            this.Confirm_Button.Name = "Confirm_Button";
            this.Confirm_Button.Size = new System.Drawing.Size(124, 23);
            this.Confirm_Button.TabIndex = 9;
            this.Confirm_Button.Text = "Confirm Payment";
            this.Confirm_Button.UseVisualStyleBackColor = true;
            this.Confirm_Button.Click += new System.EventHandler(this.Confirm_Button_Click);
            // 
            // Back_Button
            // 
            this.Back_Button.Location = new System.Drawing.Point(39, 315);
            this.Back_Button.Name = "Back_Button";
            this.Back_Button.Size = new System.Drawing.Size(75, 23);
            this.Back_Button.TabIndex = 10;
            this.Back_Button.Text = "Back";
            this.Back_Button.UseVisualStyleBackColor = true;
            this.Back_Button.Click += new System.EventHandler(this.Back_Button_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 450);
            this.Controls.Add(this.Back_Button);
            this.Controls.Add(this.Confirm_Button);
            this.Controls.Add(this.Change_Value);
            this.Controls.Add(this.Change);
            this.Controls.Add(this.Cash_input);
            this.Controls.Add(this.Cash_given);
            this.Controls.Add(this.Choose_Card_Method);
            this.Controls.Add(this.Choose_Cash_Method);
            this.Controls.Add(this.Amount_Value);
            this.Controls.Add(this.Amount_label);
            this.Controls.Add(this.Title);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Amount_label;
        private System.Windows.Forms.Label Amount_Value;
        private System.Windows.Forms.RadioButton Choose_Cash_Method;
        private System.Windows.Forms.RadioButton Choose_Card_Method;
        private System.Windows.Forms.Label Cash_given;
        private System.Windows.Forms.TextBox Cash_input;
        private System.Windows.Forms.Label Change;
        private System.Windows.Forms.Label Change_Value;
        private System.Windows.Forms.Button Confirm_Button;
        private System.Windows.Forms.Button Back_Button;
    }
}