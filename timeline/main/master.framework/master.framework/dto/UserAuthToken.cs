using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class UserAuthToken
    {
        public string IdToken { get; set; }
        public string IdUser { get; set; }
        public string Login { get; set; }
        public string IdSystem { get; set; }
        public DateTime Generated { get; set; }
    }
}
