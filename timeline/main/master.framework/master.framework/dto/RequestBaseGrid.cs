using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class RequestBaseGrid : RequestBase
    {
        public bool OnlyCount { get; set; }
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string Sort { get; set; } = "ID ASC";
    }
}
