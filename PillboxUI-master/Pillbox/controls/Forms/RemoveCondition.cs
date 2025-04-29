using Pillbox.entries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pillbox.controls.Forms
{
    public partial class RemoveCondition : Form
    {
        private List<Condition> conditions = new List<Condition>();
        private List<Condition> allergies = new List<Condition>();
        private List<Condition> intolerances = new List<Condition>();
        private List<Condition> restrictions = new List<Condition>();


        public RemoveCondition()
        {
            InitializeComponent();
            updateCollection();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void updateCollection()
        {

            SqliteDataAccess.LoadAll<Condition>("Conditions").ForEach(x =>
            {
                if (x.Type == "ALLERGY")
                {
                    if (!comboBoxType.Items.Contains("ALLERGY"))
                    {
                        comboBoxType.Items.Add("ALLERGY");
                    }
                    allergies.Add(x);
                }
                else if (x.Type == "INTOLERANCE")
                {
                    if (!comboBoxType.Items.Contains("INTOLERANCE"))
                    {
                        comboBoxType.Items.Add("INTOLERANCE");
                    }
                    intolerances.Add(x);
                }
                else if (x.Type == "RESTRICTION")
                {
                    if (!comboBoxType.Items.Contains("RESTRICTION"))
                    {
                        comboBoxType.Items.Add("RESTRICTION");
                    }
                    restrictions.Add(x);
                }
                else if (x.Type == "CONDITION")
                {
                    if (!comboBoxType.Items.Contains("CONDITION"))
                    {
                        comboBoxType.Items.Add("CONDITION");
                    }
                    conditions.Add(x);
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selected = comboBoxCondition.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selected))
            {
                MessageBox.Show("Please select an entry to delete.");
                return;
            }
            DialogResult result = MessageBox.Show("Are you sure you want to delete this Entry?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SqliteDataAccess.DeleteEntry<Condition, string>("CONDITIONS", "NAME", selected);
                MessageBox.Show("Medication Deleted");
                this.Close();
            }
            else
            {
                MessageBox.Show("Medication not deleted");
                this.Close();
            }
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxType.SelectedItem.ToString() == "ALLERGY")
            {
                comboBoxCondition.Items.Clear();
                foreach (Condition entry in allergies){comboBoxCondition.Items.Add(entry.Name);}   
            }
            else if (comboBoxType.SelectedItem.ToString() == "INTOLERANCE")
            {
                comboBoxCondition.Items.Clear();
                foreach (Condition entry in intolerances) { comboBoxCondition.Items.Add(entry.Name); }
            }
            else if (comboBoxType.SelectedItem.ToString() == "RESTRICTION")
            {
                comboBoxCondition.Items.Clear();
                foreach (Condition entry in restrictions) { comboBoxCondition.Items.Add(entry.Name); }
            }
            else if (comboBoxType.SelectedItem.ToString() == "CONDITION")
            {
                comboBoxCondition.Items.Clear();
                foreach (Condition entry in conditions) { comboBoxCondition.Items.Add(entry.Name); }
            }
        }
    }
}


