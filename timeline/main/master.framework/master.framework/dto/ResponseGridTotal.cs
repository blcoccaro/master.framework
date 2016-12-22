using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class ResponseGridTotal : ResponseBase
    {
        public TimeStampKeeper Timestamp { get; set; } = new TimeStampKeeper();
        public int TotalRows { get; set; }
    }
}
