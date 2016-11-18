using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.attribute.database
{
    public class Updated : System.Attribute
    {
        public master.framework.Enumerators.UpdatedType UpdatedType { get; set; }
        public Updated(master.framework.Enumerators.UpdatedType updatedType = Enumerators.UpdatedType.Date)
        {
            UpdatedType = updatedType;
        }

    }
}
