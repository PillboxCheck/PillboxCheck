namespace Pillbox.controls
{
    partial class Chat
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chat));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panel1 = new Panel();
            sendButton = new Guna.UI2.WinForms.Guna2ImageButton();
            QuestionTextBox = new Guna.UI2.WinForms.Guna2TextBox();
            guna2Separator1 = new Guna.UI2.WinForms.Guna2Separator();
            panel2 = new Panel();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(sendButton);
            panel1.Controls.Add(QuestionTextBox);
            panel1.Controls.Add(guna2Separator1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 649);
            panel1.Name = "panel1";
            panel1.Size = new Size(1582, 158);
            panel1.TabIndex = 0;
            // 
            // sendButton
            // 
            sendButton.CheckedState.ImageSize = new Size(64, 64);
            sendButton.HoverState.ImageSize = new Size(64, 64);
            sendButton.Image = (Image)resources.GetObject("sendButton.Image");
            sendButton.ImageOffset = new Point(0, 0);
            sendButton.ImageRotate = 0F;
            sendButton.Location = new Point(1489, 42);
            sendButton.Name = "sendButton";
            sendButton.PressedState.ImageSize = new Size(64, 64);
            sendButton.ShadowDecoration.CustomizableEdges = customizableEdges1;
            sendButton.Size = new Size(80, 68);
            sendButton.TabIndex = 2;
            sendButton.Click += sendButton_Click;
            // 
            // QuestionTextBox
            // 
            QuestionTextBox.AcceptsTab = true;
            QuestionTextBox.AllowDrop = true;
            QuestionTextBox.BorderRadius = 20;
            QuestionTextBox.CustomizableEdges = customizableEdges2;
            QuestionTextBox.DefaultText = "";
            QuestionTextBox.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            QuestionTextBox.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            QuestionTextBox.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            QuestionTextBox.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            QuestionTextBox.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            QuestionTextBox.Font = new Font("Century Gothic", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            QuestionTextBox.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            QuestionTextBox.Location = new Point(36, 17);
            QuestionTextBox.Margin = new Padding(4);
            QuestionTextBox.MaxLength = 10000;
            QuestionTextBox.Multiline = true;
            QuestionTextBox.Name = "QuestionTextBox";
            QuestionTextBox.PlaceholderText = "Please enter your query here";
            QuestionTextBox.SelectedText = "";
            QuestionTextBox.ShadowDecoration.CustomizableEdges = customizableEdges3;
            QuestionTextBox.Size = new Size(1446, 123);
            QuestionTextBox.TabIndex = 1;
            // 
            // guna2Separator1
            // 
            guna2Separator1.BackColor = Color.Transparent;
            guna2Separator1.Dock = DockStyle.Top;
            guna2Separator1.Location = new Point(0, 0);
            guna2Separator1.Name = "guna2Separator1";
            guna2Separator1.Size = new Size(1582, 10);
            guna2Separator1.TabIndex = 0;
            guna2Separator1.UseTransparentBackground = true;
            // 
            // panel2
            // 
            panel2.AutoScroll = true;
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1582, 649);
            panel2.TabIndex = 1;
            panel2.Paint += panel2_Paint;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // Chat
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "Chat";
            Size = new Size(1582, 807);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Guna.UI2.WinForms.Guna2TextBox QuestionTextBox;
        private Guna.UI2.WinForms.Guna2Separator guna2Separator1;
        private Guna.UI2.WinForms.Guna2ImageButton sendButton;
        private Panel panel2;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
    }
}
