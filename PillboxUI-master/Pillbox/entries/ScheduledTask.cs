using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox.entries
{
    public class ScheduledTask
    {
        public int Id { get; set; }  // For DB primary key (optional)
        public string Name { get; set; }
        public DateTime ScheduledTime { get; set; }

        public string Location { get; set; }

        public string ScheduledTimeSTR
        {
            get => ScheduledTime.ToString("yyyy-MM-dd HH:mm:ss"); // Optional format
            set
            {
                if (DateTime.TryParse(value, out DateTime parsed))
                {
                    ScheduledTime = parsed;
                }
            }
        }
        public bool IsMedication { get; set; }
        public bool IsTriggered { get; set; }

        public RepeatPattern Repeat { get; set; } = RepeatPattern.None;
        public TimeSpan? RepeatInterval { get; set; }

        public string IntervalSpan
        {
            get => RepeatInterval == null
                ? null
                : string.Format("{0}:{1:D2}:{2:D2}:{3:D2}",
                    RepeatInterval.Value.Days,
                    RepeatInterval.Value.Hours,
                    RepeatInterval.Value.Minutes,
                    RepeatInterval.Value.Seconds);

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var parts = value.Split(':');
                    if (parts.Length == 4 &&
                        int.TryParse(parts[0], out int days) &&
                        int.TryParse(parts[1], out int hours) &&
                        int.TryParse(parts[2], out int minutes) &&
                        int.TryParse(parts[3], out int seconds))
                    {
                        RepeatInterval = new TimeSpan(days, hours, minutes, seconds);
                        //MessageBox.Show($"Timespan {RepeatInterval.Value.TotalSeconds}");
                        return;
                    }
                }

                RepeatInterval = null; // Fallback if parsing fails
            }
        }


        public enum RepeatPattern
        {
            None,
            Daily,
            Weekly,
            Monthly,
            Yearly,
            Meal,
            Custom
        }

        public void Reschedule()
        {
            if (Repeat == RepeatPattern.None)
                return;

            var now = DateTime.Now;

            while (ScheduledTime <= now)
            {
                switch (Repeat)
                {
                    case RepeatPattern.Daily:
                        ScheduledTime = ScheduledTime.AddDays(1);
                        break;
                    case RepeatPattern.Weekly:
                        ScheduledTime = ScheduledTime.AddDays(7);
                        break;
                    case RepeatPattern.Monthly:
                        ScheduledTime = ScheduledTime.AddMonths(1);
                        break;
                    case RepeatPattern.Yearly:
                        ScheduledTime = ScheduledTime.AddYears(1);
                        break;
                    case RepeatPattern.Meal:
                        ScheduledTime = ScheduledTime.AddHours(8);
                        break;
                    case RepeatPattern.Custom:
                        if (RepeatInterval.HasValue)
                        {
                            ScheduledTime = ScheduledTime.Add(RepeatInterval.Value);
                        }
                        else
                        {
                            return; // Can't reschedule without interval
                        }
                        break;
                }
            }

            IsTriggered = false;
        }

    }


}
