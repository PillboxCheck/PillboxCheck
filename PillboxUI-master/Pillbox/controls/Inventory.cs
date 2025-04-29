using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pillbox.controls.Forms;
using KokoroSharp;
using Pillbox.entries;
using Pillbox.Managers;

namespace Pillbox.controls
{
    public partial class Inventory : UserControl
    {
        private CommunicationManager _commsManager;
        private TaskManager _taskManager;
        public Inventory(CommunicationManager commsManager, TaskManager taskManager)
        {
            InitializeComponent();
            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;
            _commsManager = commsManager;
            _taskManager = taskManager;
            LoadData();
        }

        private void LoadData()
        {
            var data = SqliteDataAccess.LoadAll<MedicationEntry>("MEDICATIONS");

            DateTime now = DateTime.Now;

            var filteredData = data.Where(x => (x.IsOngoing || DateTime.Parse(x.EndDate) >= now) && now <= DateTime.Parse(x.ExpDate)).ToList();
            dataGridView1.DataSource = new BindingSource { DataSource = filteredData };

            dataGridView1.Columns["MedicationName"].HeaderText = "Medication Name";
            dataGridView1.Columns["Dose"].HeaderText = "Dose";
            dataGridView1.Columns["DoseUnit"].HeaderText = "Dose Unit";
            dataGridView1.Columns["StartDate"].HeaderText = "Start Date";
            dataGridView1.Columns["ExpDate"].HeaderText = "Expiry Date";
            dataGridView1.Columns["Stock"].HeaderText = "Stock";
            dataGridView1.Columns["Instruction"].HeaderText = "Instruction";
            dataGridView1.Columns["IsOngoing"].HeaderText = "Ongoing";
            dataGridView1.Columns["EndDate"].HeaderText = "End Date";


            dataGridView1.Columns["Quantity"].Visible = false;
            dataGridView1.Columns["Period"].Visible = false;
            dataGridView1.Columns["Times"].Visible = false;
            dataGridView1.Columns["BoxCount"].Visible = false;
            dataGridView1.Columns["Restock"].Visible = false;
            dataGridView1.Columns["PatientName"].Visible = false;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;         
            //dataGridView1.Width = dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);


            var pastFilteredData = data.Where(x => !x.IsOngoing && DateTime.Compare(DateTime.Parse(x.EndDate), DateTime.Now) < 0 && now <= DateTime.Parse(x.ExpDate)).ToList();
            dataGridView2.DataSource = new BindingSource { DataSource = pastFilteredData };

            dataGridView2.Columns["MedicationName"].HeaderText = "Medication Name";
            dataGridView2.Columns["Dose"].HeaderText = "Dose";
            dataGridView2.Columns["DoseUnit"].HeaderText = "Dose Unit";
            dataGridView2.Columns["StartDate"].HeaderText = "Start Date";
            dataGridView2.Columns["ExpDate"].HeaderText = "Expiry Date";
            dataGridView2.Columns["Stock"].HeaderText = "Stock";
            dataGridView2.Columns["Instruction"].HeaderText = "Instruction";
            dataGridView2.Columns["IsOngoing"].HeaderText = "Ongoing";
            dataGridView2.Columns["EndDate"].HeaderText = "End Date";



            dataGridView2.Columns["Quantity"].Visible = false;
            dataGridView2.Columns["Period"].Visible = false;
            dataGridView2.Columns["Times"].Visible = false;
            dataGridView2.Columns["BoxCount"].Visible = false;
            dataGridView2.Columns["Restock"].Visible = false;
            dataGridView2.Columns["PatientName"].Visible = false;
            dataGridView2.Columns["Stock"].Visible = false;
            dataGridView2.Columns["ExpDate"].Visible = false;
            //dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            //dataGridView2.Width = dataGridView2.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);


            var expFilteredData = data.Where(x => now > DateTime.Parse(x.ExpDate)).ToList();
            dataGridExpired.DataSource = new BindingSource { DataSource = expFilteredData };

            foreach (DataGridViewColumn column in dataGridExpired.Columns)
            {
                column.Visible = false;
            }
            dataGridExpired.Columns["MedicationName"].Visible = true;
            dataGridExpired.Columns["MedicationName"].HeaderText = "Medication Name";
            dataGridExpired.Columns["ExpDate"].Visible = true;
            dataGridExpired.Columns["ExpDate"].HeaderText = "Expiry Date";
            //dataGridExpired.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridExpired.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            //dataGridExpired.Width = dataGridExpired.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);

            Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddMedication form = new AddMedication(_commsManager, _taskManager);
            form.ShowDialog();
            LoadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemoveMedication form = new RemoveMedication(_taskManager);
            form.ShowDialog();
            LoadData();
        }
    }
}
