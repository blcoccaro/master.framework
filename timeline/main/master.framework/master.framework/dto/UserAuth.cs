using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class UserAuth
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Login { get; set; }
        public string Token { get; set; }
        public DateTime? TokenValidUntil { get; set; }

        public List<string> Actions { get; set; }
    }
}
