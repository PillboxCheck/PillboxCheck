using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Pillbox.Managers;

namespace Pillbox.controls
{
    public partial class Chat : UserControl
    {
        public Chat(CommunicationManager cm)
        {
            InitializeComponent();
            CommsManager = cm;
            cm.ReplyReceivedForText += Handle_ReplyReceived;
        }

        private void Handle_ReplyReceived(object? sender, ReplyReceivedEventArgs e)
        {
            addIncomming(e.Reply);
            Refresh();
        }

        private CommunicationManager CommsManager;
        private void incomming1_Load(object sender, EventArgs e)
        {

        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void incomming1_Load_1(object sender, EventArgs e)
        {

        }

        private void outgoing1_Load(object sender, EventArgs e)
        {

        }

        private void outgoing1_Load_1(object sender, EventArgs e)
        {

        }

        void addOutgoing(string message)
        {
            var bubble = new chatItems.Outgoing();
            panel2.Controls.Add(bubble);
            bubble.Dock = DockStyle.Top;
            bubble.Message = message;
            bubble.BringToFront();
            
        }
        void addIncomming(string message)
        {
            var bubble = new chatItems.Incomming();
            panel2.Controls.Add(bubble);
            bubble.Dock = DockStyle.Top;
            bubble.Message = message;
            bubble.BringToFront();
            sendButton.Checked = false;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            Send();
        }

        void Send()
        {
            if (QuestionTextBox.Text.Trim().Length == 0) return;

            addOutgoing(QuestionTextBox.Text);
            CommsManager.SendMessage(QuestionTextBox.Text, ChannelOwner.TextChat);
            QuestionTextBox.Text = string.Empty;
            sendButton.Checked = true;
            Refresh();
        }

    }
}
