using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.selenium
{
    public enum GetElementType
    {
        Id,
        Css
    }
    public enum EnvironmentType
    {
        Localhost,
        Development,
        Prototype,
        Homologation,
        PreProduction,
        Production
    }
    public enum SearchType
    {
        Exact,
        AtEnd,
        AtStart,
        Contains,
    }
}
