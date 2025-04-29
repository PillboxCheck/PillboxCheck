using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pillbox.entries;
using Pillbox.Managers;
using Pillbox.controls.Forms;
using Pillbox.Services;

namespace Pillbox
{
    public partial class Dashboard : UserControl
    {
        private TaskManager _taskManager;
        private OverDueManager _dueManager;


        public Dashboard(TaskManager taskmanager, OverDueManager dueManager)
        {
            InitializeComponent();
            _taskManager = taskmanager;
            RefreshPersonalDetailsLabels();
            RefreshMeds();
            _taskManager.TaskDue += Handle_TaskDueDashboard;
            _dueManager = dueManager;
            _dueManager.NewOverdue += Handle_TaskOverdue;
            UpdateLastNext();
            RefreshOverdue();
            RefreshEvents();
        }

        private void Handle_TaskOverdue(object sender, OverdueTask task)
        {
            RefreshOverdue();
        }

        private void RefreshOverdue()
        {
            List<OverdueTask> tasks = _dueManager.overdueTasks;
            dataGridViewOverdue.DataSource = null;
            dataGridViewOverdue.DataSource = tasks;
            dataGridViewOverdue.Columns["Id"].Visible = false;
            dataGridViewOverdue.Columns["ScheduledTime"].Visible = false;
            dataGridViewOverdue.Columns["Name"].ReadOnly = true;
            dataGridViewOverdue.Columns["ScheduledTimeSTR"].ReadOnly = true;
            dataGridViewOverdue.Columns["ScheduledTimeSTR"].HeaderText = "Overdue Since";
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
        private void RefreshPersonalDetailsLabels()
        {
            Persona p = SqliteDataAccess.LoadAll<Persona>("PERSONAL_DETAILS").First();
            labelName.Text = p.Name;
            labelLastName.Text = p.LastName;
            labelDoB.Text = p.DoB;
            labelAge.Text = p.Age.ToString() + " Years old";
            labelGender.Text = p.Gender;
            labelHeight.Text = p.Height.ToString("F2") + " cm";
            labelWeight.Text = p.Weight.ToString("F2") + " Kg";
        }


        private void RefreshMeds()
        {
            List<MedicationEntry> list = new List<MedicationEntry>();
            List<MedicationEntry> restockList = new List<MedicationEntry>();
            List<MedicationEntry> meds = SqliteDataAccess.LoadAll<MedicationEntry>("MEDICATIONS");
            if (meds.Count > 0)
            {
                foreach (MedicationEntry med in meds)
                {
                    if (med.IsOngoing || DateTime.Parse(med.EndDate)> DateTime.Now)
                    {
                        if (DateTime.Parse(med.ExpDate) < DateTime.Now) { med.MedicationName += "  (EXPIRED)"; }
                        list.Add(med);
                        
                    }
                    if (med.Restock)
                    {
                        restockList.Add(med);
                    }
                }
                if (list.Count > 0)
                {

                    CurrentMedGridView.DataSource = list;
                    ConfigureSingleColumnDisplay(CurrentMedGridView, "MedicationName");
                }
                else
                {
                    if (CurrentMedGridView.Rows.Count > 0)
                    {
                        CurrentMedGridView.DataSource = null;

                    }
                }
                if (restockList.Count > 0)
                {
                    RestockGridView.DataSource = restockList;
                    ConfigureSingleColumnDisplay(RestockGridView, "MedicationName");
                }
                else
                {
                    if (RestockGridView.Rows.Count > 0)
                    {
                        RestockGridView.DataSource = null;
                    }
                }
            }
        }

        private void Handle_TaskDueDashboard(object sender, ScheduledTask task)
        {
            UpdateLastNext();
            RefreshEvents();
        }

        private void ConfigureSingleColumnDisplay(DataGridView grid, string columnName)
        {
            foreach (DataGridViewColumn column in grid.Columns)
                column.Visible = false;
            if (grid.Columns.Contains(columnName))
            {
                grid.Columns[columnName].Visible = true;
                grid.Columns[columnName].Width = grid.Width - 1;
            }
        }

        private void UpdateLastNext()
        {
            List<ScheduledTask> lastTasks = _taskManager.LastTakenMedicationTasks;
            if (lastTasks != null)
            {
                dataGridViewLastTake.DataSource = lastTasks;
                ConfigureSingleColumnDisplay(dataGridViewLastTake, "Name");
                LastLableTime.Text = lastTasks.FirstOrDefault()?.ScheduledTime.ToString("HH:mm");
            }
            List<ScheduledTask> nextTasks = _taskManager.NextDueMedicationTasks;
            if (nextTasks != null)
            {
                dataGridViewFuture.DataSource = nextTasks;
                ConfigureSingleColumnDisplay(dataGridViewFuture, "Name");
                NextTimeLbl.Text = nextTasks.FirstOrDefault()?.ScheduledTime.ToString("HH:mm");
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            PasswordForm f = new PasswordForm();
            DialogResult result = _dueManager.AdminMode ? f.ShowDialog() : DialogResult.OK;

            if (result == DialogResult.OK)
            {
                // Get taken tasks
                List<OverdueTask> tasks = _dueManager.overdueTasks;
                var takenTasks = tasks.Where(t => t.Taken).ToList();

                foreach (var task in takenTasks)
                {
                    // Remove from overdue list
                    bool processed = _dueManager.processStock(task);
                    if (processed) { _dueManager.RemoveTask(task); }

                }

                // Refresh grid
                RefreshOverdue();

                MessageBox.Show("Medication(s) confirmed.");
            }
            else
            {
                MessageBox.Show("Sorry, could not confirm medications.");
            }
        }

        private void summaryButton_Click(object sender, EventArgs e)
        {
            SummaryPDF.GenerateSummaryPDF();
        }

        private void RefreshEvents()
        {
            List<ScheduledTask> rawtasks = SqliteDataAccess.LoadAll<ScheduledTask>("TASKS");
            List<ScheduledTask> filtered = rawtasks.Where(x => !x.IsMedication).ToList();
            dataGridEvents.DataSource = filtered;
            foreach (DataGridViewColumn column in dataGridEvents.Columns)
                column.Visible = false;
            ScheduledTask s = new ScheduledTask();

            dataGridEvents.Columns["Name"].Visible = true;
            dataGridEvents.Columns["ScheduledTimeSTR"].Visible = true;
            dataGridEvents.Columns["ScheduledTimeSTR"].HeaderText = "Date and Time";
            dataGridEvents.Columns["Location"].Visible = true;

            dataGridEvents.ReadOnly = true;


        }

  
    }
}
