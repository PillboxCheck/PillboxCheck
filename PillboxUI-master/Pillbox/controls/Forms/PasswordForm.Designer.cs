namespace Pillbox.controls.Forms
{
    partial class PasswordForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            textBoxPassword = new TextBox();
            labelInstruction = new Label();
            EnterButton = new Button();
            CancelButton = new Button();
            ToggleShow = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            label1 = new Label();
            SuspendLayout();
            // 
            // textBoxPassword
            // 
            textBoxPassword.Font = new Font("Century Gothic", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxPassword.Location = new Point(25, 108);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(484, 36);
            textBoxPassword.TabIndex = 0;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // labelInstruction
            // 
            labelInstruction.AutoSize = true;
            labelInstruction.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelInstruction.Location = new Point(25, 29);
            labelInstruction.Name = "labelInstruction";
            labelInstruction.Size = new Size(388, 23);
            labelInstruction.TabIndex = 1;
            labelInstruction.Text = "Please enter the password to continue";
            // 
            // EnterButton
            // 
            EnterButton.Font = new Font("Century Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            EnterButton.Location = new Point(459, 188);
            EnterButton.Name = "EnterButton";
            EnterButton.Size = new Size(94, 29);
            EnterButton.TabIndex = 2;
            EnterButton.Text = "Enter";
            EnterButton.UseVisualStyleBackColor = true;
            EnterButton.Click += EnterButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Font = new Font("Century Gothic", 10.8F);
            CancelButton.Location = new Point(25, 188);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(94, 29);
            CancelButton.TabIndex = 3;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            CancelButton.Click += CancelButton_Click;
            // 
            // ToggleShow
            // 
            ToggleShow.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            ToggleShow.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            ToggleShow.CheckedState.InnerBorderColor = Color.White;
            ToggleShow.CheckedState.InnerColor = Color.White;
            ToggleShow.CustomizableEdges = customizableEdges1;
            ToggleShow.Location = new Point(526, 108);
            ToggleShow.Name = "ToggleShow";
            ToggleShow.ShadowDecoration.CustomizableEdges = customizableEdges2;
            ToggleShow.Size = new Size(44, 25);
            ToggleShow.TabIndex = 4;
            ToggleShow.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            ToggleShow.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            ToggleShow.UncheckedState.InnerBorderColor = Color.White;
            ToggleShow.UncheckedState.InnerColor = Color.White;
            ToggleShow.CheckedChanged += ToggleShow_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(527, 136);
            label1.Name = "label1";
            label1.Size = new Size(43, 20);
            label1.TabIndex = 5;
            label1.Text = "show";
            // 
            // PasswordForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 247);
            Controls.Add(label1);
            Controls.Add(ToggleShow);
            Controls.Add(CancelButton);
            Controls.Add(EnterButton);
            Controls.Add(labelInstruction);
            Controls.Add(textBoxPassword);
            Name = "PasswordForm";
            Text = "PasswordForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxPassword;
        private Label labelInstruction;
        private Button EnterButton;
        private Button CancelButton;
        private Guna.UI2.WinForms.Guna2ToggleSwitch ToggleShow;
        private Label label1;
    }
}