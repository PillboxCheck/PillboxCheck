using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pillbox.entries
{
    class SecurityEntry
    {
        public string Hash { get; set; }
        public int Id { get; set; }
        public string Salt { get; set; }

    }
}
