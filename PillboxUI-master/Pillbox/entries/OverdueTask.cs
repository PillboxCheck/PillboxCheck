using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pillbox.entries.ScheduledTask;

namespace Pillbox.entries
{
    public class OverdueTask
    {
        public int Id { get; set; }  // For DB primary key (optional)
        public string Name { get; set; }
        public DateTime ScheduledTime { get; set; }

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
        public bool Taken { get; set; }


    }
}
