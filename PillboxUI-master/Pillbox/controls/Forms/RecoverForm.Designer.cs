namespace Pillbox.controls.Forms
{
    partial class RecoverForm
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            ToggleShow = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            CancelButton = new Button();
            EnterButton = new Button();
            labelInstruction = new Label();
            textBoxPassword = new TextBox();
            label2 = new Label();
            ToggleShow1 = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            LabelRecover = new Label();
            textBoxRecover = new TextBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(540, 282);
            label1.Name = "label1";
            label1.Size = new Size(43, 20);
            label1.TabIndex = 11;
            label1.Text = "show";
            // 
            // ToggleShow
            // 
            ToggleShow.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            ToggleShow.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            ToggleShow.CheckedState.InnerBorderColor = Color.White;
            ToggleShow.CheckedState.InnerColor = Color.White;
            ToggleShow.CustomizableEdges = customizableEdges1;
            ToggleShow.Location = new Point(539, 254);
            ToggleShow.Name = "ToggleShow";
            ToggleShow.ShadowDecoration.CustomizableEdges = customizableEdges2;
            ToggleShow.Size = new Size(44, 25);
            ToggleShow.TabIndex = 10;
            ToggleShow.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            ToggleShow.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            ToggleShow.UncheckedState.InnerBorderColor = Color.White;
            ToggleShow.UncheckedState.InnerColor = Color.White;
            ToggleShow.CheckedChanged += ToggleShow_CheckedChanged;
            // 
            // CancelButton
            // 
            CancelButton.Font = new Font("Century Gothic", 10.8F);
            CancelButton.Location = new Point(38, 321);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(94, 29);
            CancelButton.TabIndex = 9;
            CancelButton.Text = "Cancel";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // EnterButton
            // 
            EnterButton.Font = new Font("Century Gothic", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            EnterButton.Location = new Point(473, 321);
            EnterButton.Name = "EnterButton";
            EnterButton.Size = new Size(94, 29);
            EnterButton.TabIndex = 8;
            EnterButton.Text = "Enter";
            EnterButton.UseVisualStyleBackColor = true;
            EnterButton.Click += EnterButton_Click;
            // 
            // labelInstruction
            // 
            labelInstruction.AutoSize = true;
            labelInstruction.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelInstruction.Location = new Point(38, 197);
            labelInstruction.Name = "labelInstruction";
            labelInstruction.Size = new Size(328, 23);
            labelInstruction.TabIndex = 7;
            labelInstruction.Text = "Please enter the New password:";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Font = new Font("Century Gothic", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxPassword.Location = new Point(38, 243);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(484, 36);
            textBoxPassword.TabIndex = 6;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(540, 154);
            label2.Name = "label2";
            label2.Size = new Size(43, 20);
            label2.TabIndex = 17;
            label2.Text = "show";
            // 
            // ToggleShow1
            // 
            ToggleShow1.CheckedState.BorderColor = Color.FromArgb(94, 148, 255);
            ToggleShow1.CheckedState.FillColor = Color.FromArgb(94, 148, 255);
            ToggleShow1.CheckedState.InnerBorderColor = Color.White;
            ToggleShow1.CheckedState.InnerColor = Color.White;
            ToggleShow1.CustomizableEdges = customizableEdges3;
            ToggleShow1.Location = new Point(539, 126);
            ToggleShow1.Name = "ToggleShow1";
            ToggleShow1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            ToggleShow1.Size = new Size(44, 25);
            ToggleShow1.TabIndex = 16;
            ToggleShow1.UncheckedState.BorderColor = Color.FromArgb(125, 137, 149);
            ToggleShow1.UncheckedState.FillColor = Color.FromArgb(125, 137, 149);
            ToggleShow1.UncheckedState.InnerBorderColor = Color.White;
            ToggleShow1.UncheckedState.InnerColor = Color.White;
            ToggleShow1.CheckedChanged += guna2ToggleSwitch1_CheckedChanged;
            // 
            // LabelRecover
            // 
            LabelRecover.AutoSize = true;
            LabelRecover.Font = new Font("Century Gothic", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LabelRecover.Location = new Point(38, 47);
            LabelRecover.Name = "LabelRecover";
            LabelRecover.Size = new Size(435, 23);
            LabelRecover.TabIndex = 13;
            LabelRecover.Text = "Please enter the City where you were born:";
            // 
            // textBoxRecover
            // 
            textBoxRecover.Font = new Font("Century Gothic", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxRecover.Location = new Point(38, 115);
            textBoxRecover.Name = "textBoxRecover";
            textBoxRecover.Size = new Size(484, 36);
            textBoxRecover.TabIndex = 12;
            textBoxRecover.UseSystemPasswordChar = true;
            // 
            // RecoverForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(632, 362);
            Controls.Add(label2);
            Controls.Add(ToggleShow1);
            Controls.Add(LabelRecover);
            Controls.Add(textBoxRecover);
            Controls.Add(label1);
            Controls.Add(ToggleShow);
            Controls.Add(CancelButton);
            Controls.Add(EnterButton);
            Controls.Add(labelInstruction);
            Controls.Add(textBoxPassword);
            Name = "RecoverForm";
            Text = "RecoverForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Guna.UI2.WinForms.Guna2ToggleSwitch ToggleShow;
        private Button CancelButton;
        private Button EnterButton;
        private Label labelInstruction;
        private TextBox textBoxPassword;
        private Label label2;
        private Guna.UI2.WinForms.Guna2ToggleSwitch ToggleShow1;
        private Label LabelRecover;
        private TextBox textBoxRecover;
    }
}