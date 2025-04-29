using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using Pillbox.entries;

namespace Pillbox.Managers
{
    public class EventScanResult
    {
        public List<ScheduledTask> Events { get; set; }
    }
    public class TaskManager : IDisposable
    {
        public DateTime lunchTime { get; set; } = DateTime.MinValue;
        public DateTime dinnerTime { get; set; } = DateTime.MinValue;
        public DateTime breakfastTime { get; set; } = DateTime.MinValue;

        private readonly System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.Timer _cleanupTimer; //= new System.Windows.Forms.Timer();

        private readonly List<ScheduledTask> _tasks;
        public event EventHandler<ScheduledTask> TaskDue; 
        public event EventHandler<ScheduledTask> ExpiredTask;


        public List<ScheduledTask> LastTakenMedicationTasks { get; private set; }
        private List<ScheduledTask> _nextDueMedicationTasks = new();
        public List<ScheduledTask> NextDueMedicationTasks => _nextDueMedicationTasks;



        public TaskManager(int checkIntervalMs = 1000)
        {
            _tasks = new List<ScheduledTask>();
            _cleanupTimer = new System.Windows.Forms.Timer();
            LoadSavedTasks();
            CleanExpiredTasks();
            _timer = new System.Windows.Forms.Timer { Interval = checkIntervalMs };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void LoadSavedTasks()
        {
            var savedTasks = SqliteDataAccess.LoadAll<ScheduledTask>("TASKS");

            _tasks.Clear();
            _tasks.AddRange(
                savedTasks
                .Where(t => t.ScheduledTime > DateTime.Now || t.Repeat != ScheduledTask.RepeatPattern.None)
                .GroupBy(t => new { t.Name, t.ScheduledTime })
                .Select(g => g.First())
            );
            foreach (var task in _tasks)
            {
                task.Reschedule();
            }

        }
        public void RemoveTask(string taskName)
        {
            var tasksToRemove = _tasks
                .Where(t => t.Name.Equals(taskName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var task in tasksToRemove)
            {
                _tasks.Remove(task);

                if (task.Id > 0)
                {
                    SqliteDataAccess.DeleteEntry<ScheduledTask, int>("TASKS", "Id", task.Id);
                }
            }
        }


        public void CleanExpiredTasks()
        {
            DateTime now = DateTime.Now;
            var expiredTasks = new List<ScheduledTask>();
            var staledTasks = new List<ScheduledTask>();

            foreach (var task in _tasks.ToList()) // ToList() to avoid modifying while iterating
            {
                if (!task.IsMedication && task.ScheduledTime < now)
                {
                    staledTasks.Add(task);
                    continue;
                }
                var med = SqliteDataAccess.FindEntry<MedicationEntry, string>("MEDICATIONS", "MedicationName", task.Name);
                if (med == null)
                {
                    expiredTasks.Add(task); // Treat missing medication as expired
                    continue;
                }

                bool isStaled = false;
                bool isExpired = false;

                if (!med.IsOngoing && DateTime.TryParse(med.EndDate, out DateTime endDate))
                {
                    if (task.ScheduledTime > endDate || now > endDate)
                    {
                        isStaled = true;
                    }
                }

                if (!string.IsNullOrEmpty(med.ExpDate) && DateTime.TryParse(med.ExpDate, out DateTime expDate))
                {
                    if (now > expDate)
                    {
                        isExpired = true;
                    }
                }

                if (isExpired)
                {
                    expiredTasks.Add(task);
                }
                else if (isStaled)
                {
                    staledTasks.Add(task);
                }
            }

            foreach (var task in expiredTasks)
            {
                ExpiredTask?.Invoke(this, task);
                _tasks.Remove(task);
                if (task.Id > 0)
                {
                    SqliteDataAccess.DeleteEntry<ScheduledTask, int>("TASKS", "Id", task.Id);
                }
            }

            foreach (var task in staledTasks)
            {
                _tasks.Remove(task);
                if (task.Id > 0)
                {
                    SqliteDataAccess.DeleteEntry<ScheduledTask, int>("TASKS", "Id", task.Id);
                }
            }

            ScheduleNextCleanup();
        }


        private void SaveTask(ScheduledTask task)
        {
            var existing = SqliteDataAccess.FindEntry<ScheduledTask, string>("TASKS", "NAME", task.Name);

            if (existing != null)
            {
                task.Id = existing.Id; // Ensure we have the right ID for updating
            }

            if (task.Id == 0)
            {
                // New task

                string fields = "NAME, SCHEDULEDTIMESTR, ISTRIGGERED, REPEAT, ISMEDICATION, INTERVALSPAN";
                string props = "@Name, @ScheduledTimeSTR, @IsTriggered, @Repeat, @IsMedication, @IntervalSpan";
                SqliteDataAccess.Save(task, "TASKS", fields, props);
            }
            else
            {
                // Existing task
                string setClause = @"
                        NAME = @Name,
                        SCHEDULEDTIMESTR = @ScheduledTimeSTR,
                        ISTRIGGERED = @IsTriggered,
                        REPEAT = @Repeat,
                        ISMEDICATION = @IsMedication,
                        INTERVALSPAN = @IntervalSpan";
                string condition = "Id = @Id";
                SqliteDataAccess.Update(task, "TASKS", setClause, condition);
            }
        }

        public void SaveAllTasks()
        {
            foreach (var task in _tasks)
            {
                // Try to find a matching task in the DB based on Name and ScheduledTime
                //var existing = SqliteDataAccess.FindEntry<ScheduledTask, object>(
                //    "TASKS",
                //    "NAME = @Name",
                //    new { task.Name}
                //);
                SaveTask(task);
            }
        }


        public void AddRecurringPillTasks(MedicationEntry entry, DateTime start)
        {
            int timesPerPeriod = entry.Times;
            TimeSpan interval;
            var pattern = ScheduledTask.RepeatPattern.None;
            string basePeriod = entry.Period.Contains(">") ? entry.Period.Split('>')[0] : entry.Period;

            switch (basePeriod.ToUpper())
            {
                case "DAY":
                case "MORNING":
                case "NOON":
                case "NIGHT":
                    pattern = ScheduledTask.RepeatPattern.Custom;
                    interval = TimeSpan.FromDays(1.0 / timesPerPeriod);
                    break;
                case "WEEK":
                    pattern = ScheduledTask.RepeatPattern.Custom;
                    interval = TimeSpan.FromDays(7.0 / timesPerPeriod);
                    break;
                case "MONTH":
                    pattern = ScheduledTask.RepeatPattern.Custom;
                    interval = TimeSpan.FromDays(30.0 / timesPerPeriod);
                    break;
                case "YEAR":
                    pattern = ScheduledTask.RepeatPattern.Custom;
                    interval = TimeSpan.FromDays(365.0 / timesPerPeriod);
                    break;
                case "MEAL":
                    pattern = ScheduledTask.RepeatPattern.Meal;
                    interval = TimeSpan.FromHours(8);
                    timesPerPeriod = 3;
                    break;
                case "CUSTOM":
                    pattern = ScheduledTask.RepeatPattern.Custom;
                    if (entry.Period.Contains(">"))
                    {
                        var timePart = entry.Period.Split('>')[1];
                        if (!TimeSpan.TryParse(timePart, out interval))
                        {
                            interval = TimeSpan.FromHours(8); // fallback
                        }
                    }
                    else
                    {
                        interval = TimeSpan.FromHours(8); // fallback
                    }
                    break;
                default:
                    pattern = ScheduledTask.RepeatPattern.Custom;
                    interval = TimeSpan.FromDays(1);
                    break;
            }

            for (int i = 0; i < timesPerPeriod; i++)
            {
                var time = start.AddTicks(interval.Ticks * i);
                AddTask(entry.MedicationName, time, pattern, interval,true);
            }
        }

        public void AddTask(string name, DateTime time, ScheduledTask.RepeatPattern repeat, TimeSpan? interval, bool isMed=false)
        {
            bool exists = _tasks.Any(t =>
                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                t.ScheduledTime == time
            );

            if (exists) return;

            var task = new ScheduledTask
            {
                Name = name,
                ScheduledTime = time,
                Repeat = repeat,
                RepeatInterval = interval,
                IsMedication = isMed
            };

            _tasks.Add(task);

            SaveTask(task);
        }

        public void AddScheduledTask(ScheduledTask scheduledTask)
        {
            _tasks.Add(scheduledTask);
            SaveTask(scheduledTask);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            var dueTasks = _tasks
                .Where(t => !t.IsTriggered && t.ScheduledTime <= now && (now - t.ScheduledTime).TotalMinutes <= 1)
                .ToList();

            var justTaken = dueTasks.Where(t => t.IsMedication);
            if (justTaken.Any())
            {
                LastTakenMedicationTasks = justTaken?.ToList() ?? new List<ScheduledTask>();
            }

            var upcoming = _tasks
                .Where(t => !t.IsTriggered && t.IsMedication && t.ScheduledTime > now)
                .OrderBy(t => t.ScheduledTime)
                .ToList();

            if (upcoming.Any())
            {
                var firstTime = upcoming[0].ScheduledTime;
                _nextDueMedicationTasks = upcoming
                    .Where(t => Math.Abs((t.ScheduledTime - firstTime).TotalMinutes) < 5)
                    .ToList();
            }
            else
            {
                _nextDueMedicationTasks = new List<ScheduledTask>();
            }

            foreach (var task in dueTasks)
            {
                task.IsTriggered = true;
                TaskDue?.Invoke(this, task);
                task.Reschedule();
            }

            CheckForNewlyExpiredTasks();
        }


        public void Dispose()
        {
            _timer?.Stop();
            SaveAllTasks();
            _timer?.Dispose();
            _cleanupTimer?.Dispose();
        }

        private void ScheduleNextCleanup()
        {
            DateTime? nextCheckTime = null;
            DateTime now = DateTime.Now;

            var uniqueMedNames = _tasks.Select(t => t.Name).Distinct().ToList();

            foreach (var name in uniqueMedNames)
            {
                var med = SqliteDataAccess.FindEntry<MedicationEntry, string>("MEDICATIONS", "MedicationName", name);
                if (med == null) continue;

                DateTime? medExpiry = null;

                if (!string.IsNullOrEmpty(med.ExpDate) && DateTime.TryParse(med.ExpDate, out DateTime expDate))
                {
                    if (expDate > now)
                        medExpiry = expDate;
                }

                if (!med.IsOngoing && DateTime.TryParse(med.EndDate, out DateTime endDate))
                {
                    if (endDate > now)
                        medExpiry = !medExpiry.HasValue ? endDate : medExpiry < endDate ? medExpiry : endDate;
                }

                if (medExpiry.HasValue)
                {
                    nextCheckTime = !nextCheckTime.HasValue
                        ? medExpiry
                        : medExpiry < nextCheckTime ? medExpiry : nextCheckTime;
                }
            }

            if (nextCheckTime.HasValue)
            {
                var msUntilNext = (int)(nextCheckTime.Value - now).TotalMilliseconds;

                if (msUntilNext < 1000)
                    msUntilNext = 1000; // Minimum delay of 1 sec to avoid rapid firing

                _cleanupTimer.Stop();
                _cleanupTimer.Interval = msUntilNext;
                _cleanupTimer.Tick -= CleanupTimer_Tick;
                _cleanupTimer.Tick += CleanupTimer_Tick;
                _cleanupTimer.Start();
            }
            else
            {
                _cleanupTimer.Stop(); // No upcoming expiry
            }
        }


        private void CleanupTimer_Tick(object sender, EventArgs e)
        {
            _cleanupTimer.Stop();
            CleanExpiredTasks();
        }
        private void CheckForNewlyExpiredTasks()
        {
            DateTime now = DateTime.Now;
            var newlyExpiredTasks = new List<ScheduledTask>();

            foreach (var task in _tasks.ToList()) // ToList() to avoid modifying the collection while iterating
            {
                var med = SqliteDataAccess.FindEntry<MedicationEntry, string>("MEDICATIONS", "MedicationName", task.Name);
                if (med == null)
                {
                    newlyExpiredTasks.Add(task);
                    continue;
                }

                bool isExpired = false;

                if (!med.IsOngoing && DateTime.TryParse(med.EndDate, out DateTime endDate))
                {
                    if (now > endDate)
                    {
                        isExpired = true;
                    }
                }

                if (!string.IsNullOrEmpty(med.ExpDate) && DateTime.TryParse(med.ExpDate, out DateTime expDate))
                {
                    if (now > expDate)
                    {
                        isExpired = true;
                    }
                }

                if (isExpired)
                {
                    newlyExpiredTasks.Add(task);
                }
            }

            foreach (var task in newlyExpiredTasks)
            {
                ExpiredTask?.Invoke(this, task);
                _tasks.Remove(task);

                if (task.Id > 0)
                {
                    SqliteDataAccess.DeleteEntry<ScheduledTask, int>("TASKS", "Id", task.Id);
                }
            }
        }

    }
}
