using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace master.framework.web.js.Codes
{
    public class BaseController : master.framework.web.BaseController
    {
        protected override void OnAuthentication(System.Web.Mvc.Filters.AuthenticationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                //if (!isLogged)
                //{
                //    filterContext.Result = new RedirectResult(master.framework.Configuration.MasterFramework.Authentication.URLNotAuthenticated);
                //}
                #region Validate Session
                //var keyAux = isLogged ? UserAuth.Session : GetUserSessionCookie();
                //if (!string.IsNullOrWhiteSpace(keyAux))
                //{
                //    if (MustCheckSession())
                //    {
                //        UserAuth = srAuthServiceClient.GetUserSession(keyAux);
                //        if (!isLogged)
                //        {
                //            RemoveCheckSessionCookie();
                //            RemoveUserSessionCookie();
                //        }
                //        else
                //        {
                //            SetUserSessionCookie();
                //            SetCheckSessionCookie();
                //        }
                //    }
                //}
                #endregion
            }
        }
    }
}
