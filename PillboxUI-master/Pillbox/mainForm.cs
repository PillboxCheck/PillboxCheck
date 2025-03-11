using Pillbox.controls;
using System.Diagnostics;
using System.Security.AccessControl;

namespace Pillbox
{
    public partial class mainForm : Form
    {

        public CommunicationManager Manager = new CommunicationManager(serverIp: "127.0.0.1", port: 51227);

        public mainForm()
        {
            InitializeComponent();
            Debug.Write(Manager.Connect());
        }
        private void addUserControl(UserControl uc)
        {
            uc.Dock = DockStyle.Fill;
            panel4.Controls.Clear();
            panel4.Controls.Add(uc);
            uc.BringToFront();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            //AI CHAT BUTTON
            Chat uc = new Chat();
            uc.CommsManager = Manager;
            addUserControl(uc);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {


        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //DASHBOARD BUTTON
            UserControl uc = new Dashboard();
            addUserControl(uc);

        }

        private void Close_Click(object sender, EventArgs e)
        {
            //manager.StopPythonProcess();
            Manager.Disconnect();
            this.Close();
        }

        private void Minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
