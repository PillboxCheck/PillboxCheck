using Pillbox.entries;
using Pillbox.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pillbox.controls.Forms
{
    public partial class RemoveMedication : Form
    {
        private TaskManager _taskManager;
        public RemoveMedication(TaskManager taskManager)
        {
            InitializeComponent();
            updateCollection();
            _taskManager = taskManager;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void updateCollection()
        {
            SqliteDataAccess.LoadAll<MedicationEntry>("MEDICATIONS").ForEach(x =>
            {
                comboBox1.Items.Add(x.MedicationName);
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selected = comboBox1.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selected))
            {
                MessageBox.Show("Please select a medication to delete.");
                return;
            }
            DialogResult result = MessageBox.Show("Are you sure you want to delete this medication?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                SqliteDataAccess.DeleteEntry<MedicationEntry,string>("MEDICATIONS", "MedicationName", selected);
                _taskManager.RemoveTask(selected);
                MessageBox.Show("Medication Deleted");
                this.Close();
            }
            else
            {
                MessageBox.Show("Medication not deleted");
                this.Close();
            }
        }
    }
}
