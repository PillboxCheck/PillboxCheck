namespace Pillbox.controls.Forms
{
    partial class AddCondition
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCondition));
            CancelButton = new Button();
            Save = new Button();
            SeverityBox = new ComboBox();
            NameBox = new TextBox();
            medicationEntryBindingSource = new BindingSource(components);
            TypeBox = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)medicationEntryBindingSource).BeginInit();
            SuspendLayout();
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(12, 157);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(94, 29);
            CancelButton.TabIndex = 29;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // Save
            // 
            Save.Location = new Point(308, 157);
            Save.Name = "Save";
            Save.Size = new Size(94, 29);
            Save.TabIndex = 28;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // SeverityBox
            // 
            SeverityBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SeverityBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            SeverityBox.FormattingEnabled = true;
            SeverityBox.Location = new Point(114, 90);
            SeverityBox.Name = "SeverityBox";
            SeverityBox.Size = new Size(288, 31);
            SeverityBox.TabIndex = 27;
            // 
            // NameBox
            // 
            NameBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            NameBox.Location = new Point(114, 52);
            NameBox.Name = "NameBox";
            NameBox.Size = new Size(288, 32);
            NameBox.TabIndex = 18;
            // 
            // medicationEntryBindingSource
            // 
            medicationEntryBindingSource.DataSource = typeof(entries.MedicationEntry);
            // 
            // TypeBox
            // 
            TypeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            TypeBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TypeBox.FormattingEnabled = true;
            TypeBox.Items.AddRange(new object[] { "CONDITION", "ALLERGY", "INTOLERANCE", "RESTRICTION" });
            TypeBox.Location = new Point(114, 15);
            TypeBox.Name = "TypeBox";
            TypeBox.Size = new Size(288, 31);
            TypeBox.TabIndex = 36;
            TypeBox.SelectedIndexChanged += TypeBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Century Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(58, 21);
            label1.TabIndex = 37;
            label1.Text = "Type:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Century Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(12, 58);
            label2.Name = "label2";
            label2.Size = new Size(69, 21);
            label2.TabIndex = 38;
            label2.Text = "Name:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Century Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(12, 95);
            label3.Name = "label3";
            label3.Size = new Size(85, 21);
            label3.TabIndex = 39;
            label3.Text = "Severity:";
            // 
            // AddCondition
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(423, 209);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(TypeBox);
            Controls.Add(CancelButton);
            Controls.Add(Save);
            Controls.Add(SeverityBox);
            Controls.Add(NameBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AddCondition";
            Text = "AddCondition";
            Load += AddCondition_Load;
            ((System.ComponentModel.ISupportInitialize)medicationEntryBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button CancelButton;
        private Button Save;
        private ComboBox SeverityBox;
        private TextBox NameBox;
        private BindingSource medicationEntryBindingSource;
        private ComboBox TypeBox;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}