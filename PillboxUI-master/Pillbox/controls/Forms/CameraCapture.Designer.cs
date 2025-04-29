namespace Pillbox.controls.Forms
{
    partial class CameraCapture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraCapture));
            pictureBox1 = new PictureBox();
            lblConfidence = new Label();
            FrontCameraButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(61, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1125, 583);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblConfidence
            // 
            lblConfidence.AutoSize = true;
            lblConfidence.Font = new Font("Century Gothic", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblConfidence.Location = new Point(29, 677);
            lblConfidence.Name = "lblConfidence";
            lblConfidence.Size = new Size(60, 21);
            lblConfidence.TabIndex = 2;
            lblConfidence.Text = "label1";
            // 
            // FrontCameraButton
            // 
            FrontCameraButton.Font = new Font("Century Gothic", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FrontCameraButton.Location = new Point(1139, 654);
            FrontCameraButton.Name = "FrontCameraButton";
            FrontCameraButton.Size = new Size(89, 66);
            FrontCameraButton.TabIndex = 3;
            FrontCameraButton.Text = "Front Camera";
            FrontCameraButton.UseVisualStyleBackColor = true;
            FrontCameraButton.Click += FrontCameraButton_Click;
            // 
            // CameraCapture
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Snow;
            ClientSize = new Size(1253, 738);
            Controls.Add(FrontCameraButton);
            Controls.Add(lblConfidence);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CameraCapture";
            Text = "CameraCapture";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblConfidence;
        private Button FrontCameraButton;
    }
}