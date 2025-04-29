using Pillbox.entries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pillbox.controls.Forms
{
    public partial class PasswordForm : Form
    {
        private int _triesLeft;
        private bool _isSave;

        public PasswordForm(bool isSave=false, int tries = 3)
        {
            InitializeComponent();
            _triesLeft = tries;
            _isSave = isSave;
            if (_isSave)
            {
                labelInstruction.Text = "Enter New password to Save:";
                EnterButton.Text = "Save";
            }
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            string enteredPassword = textBoxPassword.Text.Trim();
            if (_isSave)
            {
                PasswordHelper.HashPassword(enteredPassword);
                this.Close();
            }
            else
            {
                if (PasswordHelper.VerifyPassword(enteredPassword))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    _triesLeft--;
                    if (_triesLeft <= 0)
                    {
                        MessageBox.Show("Too many failed attempts. Access denied.");
                        this.DialogResult = DialogResult.No;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show($"Incorrect password. {_triesLeft} attempt(s) remaining.");
                        textBoxPassword.Clear();
                        textBoxPassword.Focus();
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ToggleShow_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !textBoxPassword.UseSystemPasswordChar;
        }
    }
    public static class PasswordHelper
    {
        public static void HashPassword(string password)//, out string hash, out string salt)
        {
            // Generate 16-byte salt
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] hashBytes = pbkdf2.GetBytes(32); // 256-bit hash

            // Encode both as Base64 for storage
            string hash = Convert.ToBase64String(hashBytes);
            string salt = Convert.ToBase64String(saltBytes);

            SecurityEntry entry = new SecurityEntry { Id = 1, Hash = hash, Salt = salt };
            string setClause = "HASH = @Hash, SALT = @Salt";
            string condition = "ID=@Id";
            SqliteDataAccess.Update(entry, "SECURITY", setClause, condition);

        }
        public static void HashRecover(string password)//, out string hash, out string salt)
        {
            // Generate 16-byte salt
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] hashBytes = pbkdf2.GetBytes(32); // 256-bit hash

            // Encode both as Base64 for storage
            string hash = Convert.ToBase64String(hashBytes);
            string salt = Convert.ToBase64String(saltBytes);

            SecurityEntry entry = new SecurityEntry { Id = 2, Hash = hash, Salt = salt };
            string setClause = "HASH = @Hash, SALT = @Salt";
            string condition = "ID=@Id";
            SqliteDataAccess.Update(entry, "SECURITY", setClause, condition);

        }

        public static bool VerifyPassword(string password)//, string storedHash, string storedSalt)
        {
            SecurityEntry entry = SqliteDataAccess.LoadAll<SecurityEntry>("SECURITY").FirstOrDefault();
            byte[] saltBytes = Convert.FromBase64String(entry.Salt);
            byte[] storedHashBytes = Convert.FromBase64String(entry.Hash);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(32);

            return storedHashBytes.SequenceEqual(computedHash);
        }
        public static bool VerifyRecover(string password)//, string storedHash, string storedSalt)
        {
            SecurityEntry entry = SqliteDataAccess.LoadAll<SecurityEntry>("SECURITY")[1];
            byte[] saltBytes = Convert.FromBase64String(entry.Salt);
            byte[] storedHashBytes = Convert.FromBase64String(entry.Hash);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            byte[] computedHash = pbkdf2.GetBytes(32);

            return storedHashBytes.SequenceEqual(computedHash);
        }
    }
}