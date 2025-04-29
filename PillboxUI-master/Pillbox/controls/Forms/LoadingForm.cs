using Pillbox.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pillbox.controls.Forms
{
    public partial class LoadingForm : Form
    {
        public CommunicationManager CommsManager { get; set; }
        public LoadingForm()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            lblText.Text = "Great! give me a couple of minutes while I set up.";
            this.Refresh();
            CommsManager.DebugMode = DebugToggle.Checked;
            var result = CommsManager.Connect();
            if (!result)
            {
                MessageBox.Show("Failed to connect to Granite, please launch app again");
                this.DialogResult = DialogResult.Cancel;
                this.Close(); // Optional, if you want the dialog to close after clicking
            }
            CommsManager.StartListening();
            this.DialogResult = DialogResult.OK;
            lblText.Text = "All set! Starting up...";
            this.Close(); // Optional, if you want the dialog to close after clicking
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close(); // Optional
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
