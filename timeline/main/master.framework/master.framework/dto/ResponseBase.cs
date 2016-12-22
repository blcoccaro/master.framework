using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class ResponseBase
    {
        public bool isError { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
