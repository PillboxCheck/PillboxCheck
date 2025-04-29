using Pillbox.entries;
using Pillbox.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox.Managers
{
    public class OverDueManager : IDisposable
    {
        public bool AdminMode { get; set; }

        public event EventHandler<OverdueTask> NewOverdue;

        private List<OverdueTask> _overdueTasks;
        public List<OverdueTask> overdueTasks { get => _overdueTasks; }
        public OverDueManager() {
            _overdueTasks = new List<OverdueTask>();
            LoadOverdues();
        }

        public void RemoveTask(OverdueTask task)
        {
            _overdueTasks.Remove(task);
        }

        public void AddTask(OverdueTask task)
        {
            _overdueTasks.Add(task);
            NewOverdue?.Invoke(this, task);
        }

        public bool processStock(OverdueTask task)
        {
            MedicationEntry entry = SqliteDataAccess.FindEntry<MedicationEntry,string>("MEDICATIONS", "MedicationName",task.Name);

            if (entry != null)
            {
                int newStock = entry.Stock - entry.Quantity;
                if (newStock < 0)
                {
                    MessageBox.Show($"Cannot update Medication {entry.MedicationName}. Not enough Pills in stock if you have a box please update the stock");
                    return false;
                }
                entry.Stock = newStock;
                string setClause = @"
                        DOSE        = @Dose,
                        DOSEUNIT    = @DoseUnit,
                        QUANTITY    = @Quantity,
                        PERIOD      = @Period,
                        TIMES       = @Times,
                        BOXCOUNT    = @BoxCount,
                        EXPDATE     = @ExpDate,
                        ISONGOING   = @IsOngoing,
                        STOCK       = @Stock,
                        STARTDATE   = @StartDate,
                        ENDDATE     = @EndDate,
                        RESTOCK     = @Restock
                    ";

                string condition = "MedicationName=@MedicationName";
                SqliteDataAccess.Update(entry, "MEDICATIONS", setClause, condition);
                return true;
            }
            RemoveTask(task);
            return false;
        }

        private void LoadOverdues()
        {
            _overdueTasks?.Clear();
            _overdueTasks = SqliteDataAccess.LoadAll<OverdueTask>("OVERDUE");
        }

        private void SaveTask(OverdueTask task)
        {
            var existing = SqliteDataAccess.FindEntry<OverdueTask, string>("OVERDUE", "NAME", task.Name);

            if (existing != null)
            {
                task.Id = existing.Id; // Ensure we have the right ID for updating
            }

            if (task.Id == 0)
            {
                // New task

                string fields = "NAME, SCHEDULEDTIMESTR";
                string props = "@Name, @ScheduledTimeSTR";
                SqliteDataAccess.Save(task, "OVERDUE", fields, props);
            }
            else
            {
                // Existing task
                string setClause = @"
                        NAME = @Name,
                        SCHEDULEDTIMESTR = @ScheduledTimeSTR";
                string condition = "Id = @Id";
                SqliteDataAccess.Update(task, "OVERDUE", setClause, condition);
            }
        }

        public void SaveAllTasks()
        {
            foreach (var task in _overdueTasks)
            {
                SaveTask(task);
            }
        }

        public void Dispose()
        {
            SaveAllTasks();
        }

    }
}
