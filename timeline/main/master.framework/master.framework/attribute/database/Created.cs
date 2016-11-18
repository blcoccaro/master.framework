using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.attribute.database
{
    public class Created : System.Attribute
    {
        public master.framework.Enumerators.CreatedType CreatedType { get; set; }
        public Created(master.framework.Enumerators.CreatedType createdType = Enumerators.CreatedType.Date)
        {
            CreatedType = createdType;
        }
    }
}
