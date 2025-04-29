using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pillbox.controls.Forms
{
    partial class FileBrowser
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
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(FileBrowser));
            backButton = new Button();
            goButton = new Button();
            listView1 = new ListView();
            iconList = new ImageList(components);
            filePathTextBox = new TextBox();
            label1 = new Label();
            fileNameLabel = new Label();
            label3 = new Label();
            fileTypeLabel = new Label();
            button1 = new Button();
            AddFileButton = new Button();
            SuspendLayout();
            // 
            // backButton
            // 
            backButton.Location = new Point(3, 14);
            backButton.Margin = new Padding(3, 4, 3, 4);
            backButton.Name = "backButton";
            backButton.Size = new Size(75, 29);
            backButton.TabIndex = 0;
            backButton.Text = "Back";
            backButton.UseVisualStyleBackColor = true;
            backButton.Click += backButton_Click;
            // 
            // goButton
            // 
            goButton.Location = new Point(1223, 14);
            goButton.Margin = new Padding(3, 4, 3, 4);
            goButton.Name = "goButton";
            goButton.Size = new Size(75, 29);
            goButton.TabIndex = 1;
            goButton.Text = "Go";
            goButton.UseVisualStyleBackColor = true;
            goButton.Click += goButton_Click;
            // 
            // listView1
            // 
            listView1.BorderStyle = BorderStyle.FixedSingle;
            listView1.LargeImageList = iconList;
            listView1.Location = new Point(3, 51);
            listView1.Margin = new Padding(3, 4, 3, 4);
            listView1.Name = "listView1";
            listView1.Size = new Size(1295, 686);
            listView1.SmallImageList = iconList;
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
            listView1.MouseDoubleClick += listView1_MouseDoubleClick;
            // 
            // iconList
            // 
            iconList.ColorDepth = ColorDepth.Depth32Bit;
            iconList.ImageStream = (ImageListStreamer)resources.GetObject("iconList.ImageStream");
            iconList.TransparentColor = Color.Transparent;
            iconList.Images.SetKeyName(0, "folder.png");
            iconList.Images.SetKeyName(1, "folder2.png");
            iconList.Images.SetKeyName(2, "file.png");
            iconList.Images.SetKeyName(3, "doc.png");
            iconList.Images.SetKeyName(4, "pdf.png");
            iconList.Images.SetKeyName(5, "mp3.png");
            iconList.Images.SetKeyName(6, "mp4.png");
            iconList.Images.SetKeyName(7, "exe.png");
            iconList.Images.SetKeyName(8, "unknown.png");
            iconList.Images.SetKeyName(9, "png.png");
            iconList.Images.SetKeyName(10, "folder64.png");
            // 
            // filePathTextBox
            // 
            filePathTextBox.Location = new Point(84, 16);
            filePathTextBox.Margin = new Padding(3, 4, 3, 4);
            filePathTextBox.Name = "filePathTextBox";
            filePathTextBox.Size = new Size(1133, 27);
            filePathTextBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 762);
            label1.Name = "label1";
            label1.Size = new Size(72, 20);
            label1.TabIndex = 4;
            label1.Text = "FileName";
            // 
            // fileNameLabel
            // 
            fileNameLabel.AutoSize = true;
            fileNameLabel.Location = new Point(109, 762);
            fileNameLabel.Name = "fileNameLabel";
            fileNameLabel.Size = new Size(21, 20);
            fileNameLabel.TabIndex = 5;
            fileNameLabel.Text = "--";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(188, 762);
            label3.Name = "label3";
            label3.Size = new Size(67, 20);
            label3.TabIndex = 6;
            label3.Text = "File Type";
            // 
            // fileTypeLabel
            // 
            fileTypeLabel.AutoSize = true;
            fileTypeLabel.Location = new Point(253, 762);
            fileTypeLabel.Name = "fileTypeLabel";
            fileTypeLabel.Size = new Size(21, 20);
            fileTypeLabel.TabIndex = 7;
            fileTypeLabel.Text = "--";
            // 
            // button1
            // 
            button1.Location = new Point(1211, 758);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(75, 29);
            button1.TabIndex = 8;
            button1.Text = "Delete";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // AddFileButton
            // 
            AddFileButton.Location = new Point(1130, 758);
            AddFileButton.Margin = new Padding(3, 4, 3, 4);
            AddFileButton.Name = "AddFileButton";
            AddFileButton.Size = new Size(75, 29);
            AddFileButton.TabIndex = 9;
            AddFileButton.Text = "Add File";
            AddFileButton.UseVisualStyleBackColor = true;
            AddFileButton.Click += AddFileButton_Click;
            // 
            // FileBrowser
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1298, 814);
            Controls.Add(AddFileButton);
            Controls.Add(button1);
            Controls.Add(fileTypeLabel);
            Controls.Add(label3);
            Controls.Add(fileNameLabel);
            Controls.Add(label1);
            Controls.Add(filePathTextBox);
            Controls.Add(listView1);
            Controls.Add(goButton);
            Controls.Add(backButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "FileBrowser";
            Text = "PillboxCheck-File Browser";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ImageList iconList;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fileTypeLabel;
        private Button button1;
        private Button AddFileButton;
    }
}

