using Pillbox.entries;
using System.Diagnostics.Eventing.Reader;

namespace Pillbox.controls.Forms
{
    public partial class RecoverForm : Form
    {
        private bool _isSetup;
        private List<SecurityEntry> entries;
        private int tries;
        public RecoverForm(int tries = 3)
        {
            InitializeComponent();
            entries = SqliteDataAccess.LoadAll<SecurityEntry>("SECURITY");
            _isSetup = entries.FirstOrDefault().Hash == null;   
            this.tries = tries;

        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxRecover.UseSystemPasswordChar = !textBoxRecover.UseSystemPasswordChar;
        }

        private void ToggleShow_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !textBoxPassword.UseSystemPasswordChar;
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxRecover.Text) || string.IsNullOrEmpty(textBoxPassword.Text)) { MessageBox.Show("You must fill All fields"); return; }
            else
            {
                if (_isSetup)
                {
                    DialogResult result = MessageBox.Show("Please Make sure you have written your City correctly you WON'T be able to reset this.", "Confirm", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        PasswordHelper.HashRecover(textBoxRecover.Text.Trim());
                        PasswordHelper.HashPassword(textBoxPassword.Text.Trim());
                        MessageBox.Show("Details Saved");
                        this.DialogResult = DialogResult.Yes;
                        this.Close();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (PasswordHelper.VerifyRecover(textBoxRecover.Text.Trim()))
                    {
                        PasswordHelper.HashPassword(textBoxPassword.Text.Trim());
                        MessageBox.Show("Password Updated");
                        this.DialogResult = DialogResult.Yes;
                        this.Close();
                    }
                    else
                    {
                        tries--;
                        if (tries <= 0)
                        {
                            MessageBox.Show("Access Denied");
                            this.DialogResult = DialogResult.No;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Recovery Secret word.\n Please confirm is the your City of birth in the same format you wrote it first time.\n " +
                                $"This is case Senitive. You got {tries}(s) attempts left");
                        }
                    }
                }
            }
        }
    }
}
