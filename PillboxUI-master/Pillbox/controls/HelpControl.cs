using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using OpenTK.Audio.OpenAL;
using Pillbox.controls.Forms;
using Pillbox.Managers;
using Pillbox.entries;

namespace Pillbox.controls
{
    public partial class HelpControl : UserControl
    {
        private CommunicationManager _commsManager;
        private OverDueManager _overDueManager;
        public HelpControl(CommunicationManager manager, OverDueManager dueManager)
        {
            InitializeComponent();
            _commsManager = manager;
            _overDueManager = dueManager;

            guna2ToggleSwitch2.CheckedChanged -= guna2ToggleSwitch2_CheckedChanged;
            guna2ToggleSwitch2.Checked = _commsManager.UseBiggerGranite;
            guna2ToggleSwitch2.CheckedChanged += guna2ToggleSwitch2_CheckedChanged;
            ToggleSwitch.CheckedChanged -= ToggleSwitch_CheckedChanged;
            ToggleSwitch.Checked = _commsManager.UsePubmed;
            ToggleSwitch.CheckedChanged += ToggleSwitch_CheckedChanged;
            guna2ToggleSwitch1.CheckedChanged -= guna2ToggleSwitch1_CheckedChanged;
            guna2ToggleSwitch1.Checked = _commsManager.UseRag;
            guna2ToggleSwitch1.CheckedChanged += guna2ToggleSwitch1_CheckedChanged;
            AdminModeToggle.CheckedChanged -= AdminModeToggle_CheckedChanged;
            AdminModeToggle.Checked = _overDueManager.AdminMode;
            AdminModeToggle.CheckedChanged += AdminModeToggle_CheckedChanged;
        }

        private async void ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (_commsManager.IsChannelInUse)
            {
                MessageBox.Show($"Please wait for the execution from {_commsManager.CurrentOwner} to finish before changing the Pubmed setting.");
                ToggleSwitch.CheckedChanged -= ToggleSwitch_CheckedChanged;
                ToggleSwitch.Checked = !ToggleSwitch.Checked; // Revert the toggle state
                ToggleSwitch.CheckedChanged += ToggleSwitch_CheckedChanged;
            }
            else
            {
                _commsManager.Disconnect();
                _commsManager.UsePubmed = ToggleSwitch.Checked;
                await Task.Delay(5000); // Give some time for the disconnect to complete
                bool result = _commsManager.Connect();
                MessageBox.Show(result ? "Connected to Granite" : "Failed to connect to Granite");
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This will change the way the assistant works. Are you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                _commsManager.UseRag = guna2ToggleSwitch1.Checked;
            }
            else
            {
                guna2ToggleSwitch1.CheckedChanged -= guna2ToggleSwitch1_CheckedChanged;

                guna2ToggleSwitch1.Checked = !guna2ToggleSwitch1.Checked;

                guna2ToggleSwitch1.CheckedChanged += guna2ToggleSwitch1_CheckedChanged;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private async void AddFile_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath, "RAGSOCKET\\LocalRepository");
            //openFileDialog.ShowDialog();

            FileBrowser fm = new FileBrowser("RAGSOCKET\\LocalRepository", _commsManager);
            DialogResult result = fm.ShowDialog();
            if (result == DialogResult.OK && !_commsManager.UsePubmed)
            {
                _commsManager.Disconnect();
                await Task.Delay(5000); // Give some time for the disconnect to complete
                bool connected = _commsManager.Connect();
                MessageBox.Show(connected ? "Database Updated in Model" : "Failed to connect to Granite please use the Reconnect button");
            }
            //else
            //{
            //    MessageBox.Show("No file selected.");
            //}

        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            _commsManager.Disconnect();
            await Task.Delay(5000); // Give some time for the disconnect to complete
            bool connected = _commsManager.Connect();
            MessageBox.Show(connected ? "Granite Reconnected" : "Failed to connect to Granite please use the Reconnect button");
        }

        private async void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            if (_commsManager.IsChannelInUse)
            {
                MessageBox.Show($"Please wait for the execution from {_commsManager.CurrentOwner} to finish before changing the Pubmed setting.");
                guna2ToggleSwitch2.CheckedChanged -= guna2ToggleSwitch2_CheckedChanged;
                guna2ToggleSwitch2.Checked = !guna2ToggleSwitch2.Checked; // Revert the toggle state
                guna2ToggleSwitch2.CheckedChanged += guna2ToggleSwitch2_CheckedChanged;
            }
            else
            {
                _commsManager.Disconnect();
                _commsManager.UseBiggerGranite = guna2ToggleSwitch2.Checked;
                await Task.Delay(5000); // Give some time for the disconnect to complete
                bool result = _commsManager.Connect();
                MessageBox.Show(result ? "Connected to Granite" : "Failed to connect to Granite, please use Reconnect button");
            }

        }

        private void AdminModeToggle_CheckedChanged(object sender, EventArgs e)
        {
            SecurityEntry entry = SqliteDataAccess.LoadAll<SecurityEntry>("SECURITY").FirstOrDefault();
            if (entry != null)
            {
                if (entry.Hash == null)
                {
                    MessageBox.Show("Please set Password before doing this step");
                    AdminModeToggle.CheckedChanged -= AdminModeToggle_CheckedChanged;
                    AdminModeToggle.Checked = !AdminModeToggle.Checked; // Revert the toggle state
                    AdminModeToggle.CheckedChanged += AdminModeToggle_CheckedChanged;
                    return;
                }
                else
                {
                    PasswordForm f = new PasswordForm(isSave: false);
                    DialogResult result = f.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        _overDueManager.AdminMode = AdminModeToggle.Checked;
                        return;
                    }
                }
            }
            AdminModeToggle.CheckedChanged -= AdminModeToggle_CheckedChanged;
            AdminModeToggle.Checked = !AdminModeToggle.Checked; // Revert the toggle state
            AdminModeToggle.CheckedChanged += AdminModeToggle_CheckedChanged;
            return;
        }

        private void ButtonChangePassword_Click(object sender, EventArgs e)
        {
            SecurityEntry entry = SqliteDataAccess.LoadAll<SecurityEntry>("SECURITY").FirstOrDefault();
            if (entry != null)
            {
                if (entry.Hash == null)
                {
                    MessageBox.Show("Please set up the password first."); return;
                }
                else
                {
                    PasswordForm f = new PasswordForm(isSave: false);
                    DialogResult result = f.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        PasswordForm fNew = new PasswordForm(isSave: true);
                        DialogResult result1 = fNew.ShowDialog();
                        return;
                    }
                    MessageBox.Show("If you forgot password use the Recover/Set Up button"); return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RecoverForm recoverForm = new RecoverForm();
            DialogResult r = recoverForm.ShowDialog();
        }


    }
}
