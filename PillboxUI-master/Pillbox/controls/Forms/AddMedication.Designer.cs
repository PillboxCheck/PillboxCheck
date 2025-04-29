namespace Pillbox.controls.Forms
{
    partial class AddMedication
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddMedication));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            QuantityUpDown = new NumericUpDown();
            DoseUpDown = new NumericUpDown();
            CancelButton = new Button();
            Save = new Button();
            UnitsBox = new ComboBox();
            ExpDateTimePicker = new DateTimePicker();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            MedicationNameBox = new TextBox();
            label7 = new Label();
            medicationEntryBindingSource = new BindingSource(components);
            CameraButton = new Guna.UI2.WinForms.Guna2ImageButton();
            label8 = new Label();
            PeriodBox = new ComboBox();
            TimesUpDown = new NumericUpDown();
            CountUpDown = new NumericUpDown();
            label9 = new Label();
            OngoingCheckBox = new CheckBox();
            nDaysUpDown = new NumericUpDown();
            pictureBox2 = new PictureBox();
            label10 = new Label();
            startTimePicker = new DateTimePicker();
            label11 = new Label();
            label12 = new Label();
            HoursPicker = new NumericUpDown();
            MinutesPicker = new NumericUpDown();
            label13 = new Label();
            label14 = new Label();
            ((System.ComponentModel.ISupportInitialize)QuantityUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DoseUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)medicationEntryBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TimesUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CountUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nDaysUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)HoursPicker).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MinutesPicker).BeginInit();
            SuspendLayout();
            // 
            // QuantityUpDown
            // 
            QuantityUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            QuantityUpDown.Location = new Point(228, 130);
            QuantityUpDown.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            QuantityUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            QuantityUpDown.Name = "QuantityUpDown";
            QuantityUpDown.Size = new Size(288, 32);
            QuantityUpDown.TabIndex = 31;
            QuantityUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // DoseUpDown
            // 
            DoseUpDown.DecimalPlaces = 2;
            DoseUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DoseUpDown.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            DoseUpDown.Location = new Point(228, 53);
            DoseUpDown.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            DoseUpDown.Name = "DoseUpDown";
            DoseUpDown.Size = new Size(288, 32);
            DoseUpDown.TabIndex = 30;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(13, 514);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(94, 29);
            CancelButton.TabIndex = 29;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // Save
            // 
            Save.Location = new Point(403, 514);
            Save.Name = "Save";
            Save.Size = new Size(94, 29);
            Save.TabIndex = 28;
            Save.Text = "Save";
            Save.UseVisualStyleBackColor = true;
            Save.Click += Save_Click;
            // 
            // UnitsBox
            // 
            UnitsBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            UnitsBox.FormattingEnabled = true;
            UnitsBox.Items.AddRange(new object[] { "kg", "g", "mg", "mcg", "ng", "pg", "L", "ml", "µg" });
            UnitsBox.Location = new Point(228, 91);
            UnitsBox.Name = "UnitsBox";
            UnitsBox.Size = new Size(288, 31);
            UnitsBox.TabIndex = 27;
            // 
            // ExpDateTimePicker
            // 
            ExpDateTimePicker.CalendarFont = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ExpDateTimePicker.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ExpDateTimePicker.Location = new Point(228, 323);
            ExpDateTimePicker.Name = "ExpDateTimePicker";
            ExpDateTimePicker.Size = new Size(288, 32);
            ExpDateTimePicker.TabIndex = 26;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 99);
            label6.Name = "label6";
            label6.Size = new Size(42, 20);
            label6.TabIndex = 25;
            label6.Text = "Units";
            label6.Click += label6_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 135);
            label5.Name = "label5";
            label5.Size = new Size(176, 20);
            label5.TabIndex = 24;
            label5.Text = "Number of pills per Dose";
            label5.Click += label5_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 366);
            label4.Name = "label4";
            label4.Size = new Size(120, 20);
            label4.TabIndex = 23;
            label4.Text = "Number of Days:";
            label4.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 212);
            label3.Name = "label3";
            label3.Size = new Size(54, 20);
            label3.TabIndex = 21;
            label3.Text = "Period:";
            label3.Click += label3_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 60);
            label2.Name = "label2";
            label2.Size = new Size(46, 20);
            label2.TabIndex = 20;
            label2.Text = "Dose:";
            label2.Click += label2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 21);
            label1.Name = "label1";
            label1.Size = new Size(131, 20);
            label1.TabIndex = 19;
            label1.Text = "Medication Name:";
            label1.Click += label1_Click;
            // 
            // MedicationNameBox
            // 
            MedicationNameBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MedicationNameBox.Location = new Point(228, 15);
            MedicationNameBox.Name = "MedicationNameBox";
            MedicationNameBox.Size = new Size(288, 32);
            MedicationNameBox.TabIndex = 18;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 333);
            label7.Name = "label7";
            label7.Size = new Size(89, 20);
            label7.TabIndex = 32;
            label7.Text = "Expire Date:";
            label7.Click += label7_Click;
            // 
            // CameraButton
            // 
            CameraButton.CheckedState.ImageSize = new Size(64, 64);
            CameraButton.HoverState.ImageSize = new Size(64, 64);
            CameraButton.Image = (Image)resources.GetObject("CameraButton.Image");
            CameraButton.ImageOffset = new Point(0, 0);
            CameraButton.ImageRotate = 0F;
            CameraButton.Location = new Point(208, 475);
            CameraButton.Name = "CameraButton";
            CameraButton.PressedState.ImageSize = new Size(64, 64);
            CameraButton.ShadowDecoration.CustomizableEdges = customizableEdges1;
            CameraButton.Size = new Size(80, 68);
            CameraButton.TabIndex = 34;
            CameraButton.Click += CameraButton_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(12, 173);
            label8.Name = "label8";
            label8.Size = new Size(200, 20);
            label8.TabIndex = 35;
            label8.Text = "Number of Doses per Period:";
            label8.Click += label8_Click;
            // 
            // PeriodBox
            // 
            PeriodBox.DropDownStyle = ComboBoxStyle.DropDownList;
            PeriodBox.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PeriodBox.FormattingEnabled = true;
            PeriodBox.Items.AddRange(new object[] { "DAY", "WEEK", "MONTH", "YEAR", "MORNING", "AFTERNOON", "NIGHT", "MEAL", "CUSTOM" });
            PeriodBox.Location = new Point(228, 206);
            PeriodBox.Name = "PeriodBox";
            PeriodBox.Size = new Size(288, 31);
            PeriodBox.TabIndex = 36;
            PeriodBox.SelectedIndexChanged += PeriodBox_SelectedIndexChanged;
            // 
            // TimesUpDown
            // 
            TimesUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TimesUpDown.Location = new Point(228, 168);
            TimesUpDown.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            TimesUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            TimesUpDown.Name = "TimesUpDown";
            TimesUpDown.Size = new Size(288, 32);
            TimesUpDown.TabIndex = 37;
            TimesUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // CountUpDown
            // 
            CountUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CountUpDown.Location = new Point(228, 285);
            CountUpDown.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            CountUpDown.Name = "CountUpDown";
            CountUpDown.Size = new Size(288, 32);
            CountUpDown.TabIndex = 38;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(12, 290);
            label9.Name = "label9";
            label9.Size = new Size(191, 20);
            label9.TabIndex = 39;
            label9.Text = "Number of pills in package:";
            label9.Click += label9_Click;
            // 
            // OngoingCheckBox
            // 
            OngoingCheckBox.AutoSize = true;
            OngoingCheckBox.Location = new Point(348, 399);
            OngoingCheckBox.Name = "OngoingCheckBox";
            OngoingCheckBox.Size = new Size(168, 24);
            OngoingCheckBox.TabIndex = 40;
            OngoingCheckBox.Text = "Ongoing Medication";
            OngoingCheckBox.UseVisualStyleBackColor = true;
            // 
            // nDaysUpDown
            // 
            nDaysUpDown.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            nDaysUpDown.Location = new Point(228, 361);
            nDaysUpDown.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            nDaysUpDown.Name = "nDaysUpDown";
            nDaysUpDown.Size = new Size(288, 32);
            nDaysUpDown.TabIndex = 41;
            // 
            // pictureBox2
            // 
            pictureBox2.ErrorImage = null;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.InitialImage = null;
            pictureBox2.Location = new Point(522, 9);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(47, 32);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 43;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(196, 526);
            label10.Name = "label10";
            label10.Size = new Size(104, 20);
            label10.TabIndex = 44;
            label10.Text = "Click to Scan";
            // 
            // startTimePicker
            // 
            startTimePicker.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startTimePicker.Format = DateTimePickerFormat.Time;
            startTimePicker.Location = new Point(228, 429);
            startTimePicker.Name = "startTimePicker";
            startTimePicker.ShowUpDown = true;
            startTimePicker.Size = new Size(288, 32);
            startTimePicker.TabIndex = 45;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(12, 439);
            label11.Name = "label11";
            label11.Size = new Size(80, 20);
            label11.TabIndex = 46;
            label11.Text = "Start Time:";
            label11.Click += label11_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(12, 253);
            label12.Name = "label12";
            label12.Size = new Size(82, 20);
            label12.TabIndex = 47;
            label12.Text = "Time Span:";
            label12.Click += label12_Click_1;
            // 
            // HoursPicker
            // 
            HoursPicker.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            HoursPicker.Location = new Point(228, 243);
            HoursPicker.Maximum = new decimal(new int[] { 24, 0, 0, 0 });
            HoursPicker.Name = "HoursPicker";
            HoursPicker.Size = new Size(92, 32);
            HoursPicker.TabIndex = 48;
            // 
            // MinutesPicker
            // 
            MinutesPicker.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MinutesPicker.Location = new Point(386, 243);
            MinutesPicker.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            MinutesPicker.Name = "MinutesPicker";
            MinutesPicker.Size = new Size(84, 32);
            MinutesPicker.TabIndex = 49;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(320, 248);
            label13.Name = "label13";
            label13.Size = new Size(31, 20);
            label13.TabIndex = 50;
            label13.Text = "Hrs";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(476, 248);
            label14.Name = "label14";
            label14.Size = new Size(40, 20);
            label14.TabIndex = 51;
            label14.Text = "mins";
            // 
            // AddMedication
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(564, 555);
            Controls.Add(label14);
            Controls.Add(label13);
            Controls.Add(MinutesPicker);
            Controls.Add(HoursPicker);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(startTimePicker);
            Controls.Add(label10);
            Controls.Add(pictureBox2);
            Controls.Add(nDaysUpDown);
            Controls.Add(OngoingCheckBox);
            Controls.Add(label9);
            Controls.Add(CountUpDown);
            Controls.Add(TimesUpDown);
            Controls.Add(PeriodBox);
            Controls.Add(label8);
            Controls.Add(CameraButton);
            Controls.Add(label7);
            Controls.Add(QuantityUpDown);
            Controls.Add(DoseUpDown);
            Controls.Add(CancelButton);
            Controls.Add(Save);
            Controls.Add(UnitsBox);
            Controls.Add(ExpDateTimePicker);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(MedicationNameBox);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AddMedication";
            Text = "AddMedication";
            Load += AddMedication_Load;
            ((System.ComponentModel.ISupportInitialize)QuantityUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)DoseUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)medicationEntryBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)TimesUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)CountUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)nDaysUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)HoursPicker).EndInit();
            ((System.ComponentModel.ISupportInitialize)MinutesPicker).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown QuantityUpDown;
        private NumericUpDown DoseUpDown;
        private Button CancelButton;
        private Button Save;
        private ComboBox UnitsBox;
        private DateTimePicker ExpDateTimePicker;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox MedicationNameBox;
        private Label label7;
        private BindingSource medicationEntryBindingSource;
        private Guna.UI2.WinForms.Guna2ImageButton CameraButton;
        private Label label8;
        private ComboBox PeriodBox;
        private NumericUpDown TimesUpDown;
        private NumericUpDown CountUpDown;
        private Label label9;
        private CheckBox OngoingCheckBox;
        private NumericUpDown nDaysUpDown;
        private PictureBox pictureBox2;
        private Label label10;
        private DateTimePicker startTimePicker;
        private Label label11;
        private Label label12;
        private NumericUpDown HoursPicker;
        private NumericUpDown MinutesPicker;
        private Label label13;
        private Label label14;
    }
}