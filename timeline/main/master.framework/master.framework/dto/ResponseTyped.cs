using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.dto
{
    public class ResponseTyped<T> : ResponseBase where T : class
    {
        public T Result { get; set; }
    }
}
