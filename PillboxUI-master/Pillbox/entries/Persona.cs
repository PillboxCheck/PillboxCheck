using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox.entries
{
    public class Persona
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DoB { get; set; }
        public int Age { 
            get{
                DateTime dateOfBirth = DateTime.Parse(DoB);

                DateTime currentDate = DateTime.Now;

                int age = currentDate.Year - dateOfBirth.Year;

                if (currentDate.Month < dateOfBirth.Month ||
                    (currentDate.Month == dateOfBirth.Month && currentDate.Day < dateOfBirth.Day))
                {
                    age--;
                }
                return age;
            } }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Gender { get; set; }
    }
}
