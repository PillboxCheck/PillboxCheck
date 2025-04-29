using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox.entries
{
    public class MedicationEntry
    {

        public bool IsOngoing { get; set; }
        public string MedicationName { get; set; }
        public decimal Dose { get; set; }

        public string DoseUnit { get; set; }
        public int Quantity { get; set; }
        public int Times { get; set; }

        public string Period { get; set; }
        public string Instruction { get { return Quantity.ToString() + " pills " + Times.ToString() + " times each " + Period; } }
        public int Stock { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        
        public int BoxCount { get; set; }
        public string ExpDate { get; set; }
        public bool Restock { get; set; }

        public string PatientName { get; set; } //for cleaner OCR parse and further implementation of multiple users

    }
}
