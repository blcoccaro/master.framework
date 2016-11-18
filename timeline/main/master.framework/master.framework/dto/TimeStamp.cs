using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class TimeStamp
    {
        public string Key { get; set; }
        public Enumerators.TimeStampAction Action { get; set; }
        public Enumerators.TimeStampType Type { get; set; }
        public DateTime Date { get; set; }
    }
}
