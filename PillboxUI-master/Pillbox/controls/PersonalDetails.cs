using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;
using Pillbox.controls.Forms;
using System.Security.Cryptography.X509Certificates;
using Pillbox.entries;
using System.Diagnostics;
using Pillbox.Managers;

namespace Pillbox.controls
{
    public partial class PersonalDetails : UserControl
    {
        private CommunicationManager _commsManager;
        public PersonalDetails(CommunicationManager commsManager)
        {
            InitializeComponent();
            RefreshLabels();
            dataGridAllergies.ReadOnly = true;
            dataGridIntolerances.ReadOnly = true;
            dataGridRestrictions.ReadOnly = true;
            dataGridConditions.ReadOnly = true;
            LoadConditions();
            _commsManager = commsManager;
        }


        private void EditButton_Click(object sender, EventArgs e)
        {
            PersonalDetailsEdit f2 = new PersonalDetailsEdit();
            f2.ShowDialog();
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            Persona p = SqliteDataAccess.LoadAll<Persona>("PERSONAL_DETAILS").First();
            labelNAME.Text = p.Name;
            labelLastName.Text = p.LastName;
            labelDoB.Text = p.DoB;
            labelAge.Text = p.Age.ToString() + " Years old";
            labelGender.Text = p.Gender;
            labelHeight.Text = p.Height.ToString("F2") + " cm";
            labelWeight.Text = p.Weight.ToString("F2") + " Kg";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            FileBrowser fb = new FileBrowser("RAGSOCKET\\patient_info",_commsManager,isPersonal:true);
            DialogResult dialogResult = fb.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (_commsManager.IsChannelInUse)
                {
                    MessageBox.Show("Granite is busy processing a request. Please try again later in settings to incorporte the new files in Granite");
                }
                else
                {
                    _commsManager.Disconnect();
                    await Task.Delay(5000); // Give some time for the disconnect to complete
                    bool connected = _commsManager.Connect();
                    MessageBox.Show(connected ? "Files Updated in Assistant" : "Failed to connect to Granite please use the Reconnect button in Settings");
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddCondition f2 = new AddCondition();
            f2.ShowDialog();
            LoadConditions();
        }

        private void LoadConditions()
        {
            List<Condition> conditionsRaw = SqliteDataAccess.LoadAll<Condition>("CONDITIONS");
            List<Condition> conditions = new List<Condition>();
            List<Condition> allergies = new List<Condition>();
            List<Condition> intolerances = new List<Condition>();
            List<Condition> restrictions = new List<Condition>();

            foreach (Condition condition in conditionsRaw)
            {
                if (condition.Type == "ALLERGY")
                {
                    allergies.Add(condition);
                }
                else if (condition.Type == "INTOLERANCE")
                {
                    intolerances.Add(condition);
                }
                else if (condition.Type == "RESTRICTION")
                {
                    restrictions.Add(condition);
                }
                else
                {
                    conditions.Add(condition);
                }
            }

            dataGridAllergies.DataSource = allergies;
            dataGridIntolerances.DataSource = intolerances;
            dataGridRestrictions.DataSource = restrictions;
            dataGridConditions.DataSource = conditions;

            dataGridAllergies.Columns["Type"].Visible = false;
            dataGridIntolerances.Columns["Type"].Visible = false;
            dataGridRestrictions.Columns["Type"].Visible = false;
            dataGridConditions.Columns["Type"].Visible = false;

            Refresh();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemoveCondition f2 = new RemoveCondition();
            f2.ShowDialog();
            LoadConditions();
        }
    }
}
