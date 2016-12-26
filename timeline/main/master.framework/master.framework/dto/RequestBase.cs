using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class RequestBase
    {
        public string UserId { get; set; }
        public string UserLogin { get; set; }
        public string UserEmail { get; set; }
        public List<String> ListIdUserRequest { get; set; }
    }
}
