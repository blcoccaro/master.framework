using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace master.framework.web
{
    public class AjaxAttribute : ActionMethodSelectorAttribute
    {
        private readonly bool _ajax;

        public AjaxAttribute(bool ajax)
        {
            _ajax = ajax;
        }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return _ajax == controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}
