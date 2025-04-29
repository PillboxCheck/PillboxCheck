using Pillbox.entries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pillbox.controls.Forms
{
    public partial class PersonalDetailsEdit : Form
    {
        public PersonalDetailsEdit()
        {
            InitializeComponent();
            Persona p = SqliteDataAccess.LoadAll<Persona>("PERSONAL_DETAILS").First();
            NameBox.Text = p.Name;
            LastNameBox.Text = p.LastName;
            HeightUpDown.Value = p.Height;
            WeightUpDown.Value = p.Weight;
            DateTimePicker.Value = DateTime.Parse(p.DoB);
            GenderBox.Text = p.Gender;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Persona user = new Persona();
            user.Name = NameBox.Text;
            user.LastName = LastNameBox.Text;
            user.DoB = DateTimePicker.Value.ToString("dd/MM/yyyy");
            user.Gender = GenderBox.Text;
            user.Weight = WeightUpDown.Value;
            user.Height = HeightUpDown.Value;
          

            string setClause = "NAME=@Name, LASTNAME=@LastName, DOB=@DoB, GENDER=@Gender, WEIGHT=@Weight, HEIGHT=@Height";

            SqliteDataAccess.Update(user, "PERSONAL_DETAILS", setClause, "ID=1");
            Close();

        }
    }
}
