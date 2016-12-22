using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.attribute.database
{
    public class CreatedOrUpdated : System.Attribute
    {
        public master.framework.Enumerators.CreatedOrUpdatedType CreatedType { get; set; }
        public CreatedOrUpdated(master.framework.Enumerators.CreatedOrUpdatedType createdType = Enumerators.CreatedOrUpdatedType.Date)
        {
            CreatedType = createdType;
        }
    }
}
