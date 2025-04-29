namespace Pillbox.controls.Forms
{
    partial class PersonalDetailsEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonalDetailsEdit));
            NameBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            LastNameBox = new TextBox();
            label5 = new Label();
            label6 = new Label();
            DateTimePicker = new DateTimePicker();
            GenderBox = new ComboBox();
            Save = new Button();
            CancelButton = new Button();
            HeightUpDown = new NumericUpDown();
            WeightUpDown = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)HeightUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)WeightUpDown).BeginInit();
            SuspendLayout();
            // 
            // NameBox
            // 
            NameBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            NameBox.Location = new Point(115, 36);
            NameBox.Name = "NameBox";
            NameBox.Size = new Size(288, 32);
            NameBox.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 42);
            label1.Name = "label1";
            label1.Size = new Size(52, 20);
            label1.TabIndex = 1;
            label1.Text = "Name:";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 157);
            label2.Name = "label2";
            label2.Size = new Size(57, 20);
            label2.TabIndex = 3;
            label2.Text = "Height:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 118);
            label3.Name = "label3";
            label3.Size = new Size(97, 20);
            label3.TabIndex = 5;
            label3.Text = "Date of Birth:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 80);
            label4.Name = "label4";
            label4.Size = new Size(82, 20);
            label4.TabIndex = 7;
            label4.Text = "Last Name:";
            // 
            // LastNameBox
            // 
            LastNameBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LastNameBox.Location = new Point(115, 74);
            LastNameBox.Name = "LastNameBox";
            LastNameBox.Size = new Size(288, 32);
            LastNameBox.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 195);
            label5.Name = "label5";
            label5.Size = new Size(59, 20);
            label5.TabIndex = 9;
            label5.Text = "Weight:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 233);
            label6.Name = "label6";
            label6.Size = new Size(60, 20);
            label6.TabIndex = 11;
            label6.Text = "Gender:";
            label6.Click += label6_Click;
            // 
            // DateTimePicker
            // 
            DateTimePicker.CalendarFont = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DateTimePicker.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DateTimePicker.Location = new Point(115, 112);
            DateTimePicker.Name = "DateTimePicker";
            DateTimePicker.Size = new Size(288, 32);
            DateTimePicker.TabIndex = 12;
            // 
            // GenderBox
            // 
            GenderBox.DropDownStyle = ComboBoxStyle.DropDownList;
            GenderBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            GenderBox.FormattingEnabled = true;
            GenderBox.Items.AddRange(new object[] { "Male", "Female" });
            GenderBox.Location = new Point(115, 233);
            GenderBox.Name = "GenderBox";
            GenderBox.Size = new Size(288, 31);
            GenderBox.TabIndex = 13;
            GenderBox.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // Save
            // 
            Save.Location = new Point(299, 294);
            Save.Name = "Save";
            Save.Size = new Size(94, 29);
            Save.TabIndex = 14;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(15, 294);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(94, 29);
            CancelButton.TabIndex = 15;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // HeightUpDown
            // 
            HeightUpDown.DecimalPlaces = 2;
            HeightUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            HeightUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            HeightUpDown.Location = new Point(115, 150);
            HeightUpDown.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            HeightUpDown.Name = "HeightUpDown";
            HeightUpDown.Size = new Size(288, 32);
            HeightUpDown.TabIndex = 16;
            // 
            // WeightUpDown
            // 
            WeightUpDown.DecimalPlaces = 2;
            WeightUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            WeightUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            WeightUpDown.Location = new Point(115, 190);
            WeightUpDown.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            WeightUpDown.Name = "WeightUpDown";
            WeightUpDown.Size = new Size(288, 32);
            WeightUpDown.TabIndex = 17;
            // 
            // PersonalDetailsEdit
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(432, 345);
            Controls.Add(WeightUpDown);
            Controls.Add(HeightUpDown);
            Controls.Add(CancelButton);
            Controls.Add(Save);
            Controls.Add(GenderBox);
            Controls.Add(DateTimePicker);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(LastNameBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(NameBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PersonalDetailsEdit";
            Text = "PersonalDetailsEdit";
            ((System.ComponentModel.ISupportInitialize)HeightUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)WeightUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox NameBox;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox LastNameBox;
        private Label label5;
        private Label label6;
        private DateTimePicker DateTimePicker;
        private ComboBox GenderBox;
        private Button Save;
        private Button CancelButton;
        private NumericUpDown HeightUpDown;
        private NumericUpDown WeightUpDown;
    }
}