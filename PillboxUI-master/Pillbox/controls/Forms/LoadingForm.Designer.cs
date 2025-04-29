namespace Pillbox.controls.Forms
{
    partial class LoadingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadingForm));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pictureBox1 = new PictureBox();
            lblText = new Label();
            label2 = new Label();
            AcceptButton = new Button();
            CloseButton = new Button();
            DebugToggle = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-1, 41);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(95, 135);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblText
            // 
            lblText.AllowDrop = true;
            lblText.AutoSize = true;
            lblText.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblText.Location = new Point(100, 67);
            lblText.Name = "lblText";
            lblText.Size = new Size(362, 100);
            lblText.TabIndex = 1;
            lblText.Text = resources.GetString("lblText.Text");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(100, 41);
            label2.Name = "label2";
            label2.Size = new Size(262, 23);
            label2.TabIndex = 2;
            label2.Text = "Hello! I am PillBoxCheck. ";
            // 
            // AcceptButton
            // 
            AcceptButton.Font = new Font("Century Gothic", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            AcceptButton.Location = new Point(368, 188);
            AcceptButton.Name = "AcceptButton";
            AcceptButton.Size = new Size(94, 29);
            AcceptButton.TabIndex = 3;
            AcceptButton.Text = "Accept";
            AcceptButton.UseVisualStyleBackColor = true;
            AcceptButton.Click += AcceptButton_Click;
            // 
            // CloseButton
            // 
            CloseButton.Font = new Font("Century Gothic", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CloseButton.Location = new Point(204, 188);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(94, 29);
            CloseButton.TabIndex = 4;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // DebugToggle
            // 
            DebugToggle.CheckedState.BorderColor = Color.MediumSeaGreen;
            DebugToggle.CheckedState.FillColor = Color.MediumSeaGreen;
            DebugToggle.CheckedState.InnerBorderColor = Color.White;
            DebugToggle.CheckedState.InnerColor = Color.White;
            DebugToggle.CustomizableEdges = customizableEdges1;
            DebugToggle.Location = new Point(12, 222);
            DebugToggle.Name = "DebugToggle";
            DebugToggle.ShadowDecoration.CustomizableEdges = customizableEdges2;
            DebugToggle.Size = new Size(33, 19);
            DebugToggle.TabIndex = 5;
            DebugToggle.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            DebugToggle.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            DebugToggle.UncheckedState.InnerBorderColor = Color.White;
            DebugToggle.UncheckedState.InnerColor = Color.White;
            // 
            // label1
            // 
            label1.AllowDrop = true;
            label1.AutoSize = true;
            label1.Font = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(51, 222);
            label1.Name = "label1";
            label1.Size = new Size(108, 20);
            label1.TabIndex = 6;
            label1.Text = "Debug Mode";
            label1.Click += label1_Click;
            // 
            // LoadingForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(493, 250);
            Controls.Add(label1);
            Controls.Add(DebugToggle);
            Controls.Add(CloseButton);
            Controls.Add(AcceptButton);
            Controls.Add(label2);
            Controls.Add(lblText);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "LoadingForm";
            Text = "LoadingForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblText;
        private Label label2;
        private Button AcceptButton;
        private Button CloseButton;
        private Guna.UI2.WinForms.Guna2ToggleSwitch DebugToggle;
        private Label label1;
    }
}