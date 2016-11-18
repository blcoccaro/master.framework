using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace master.framework.web
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CheckAuthorizeAttribute : AuthorizeAttribute
    {
        List<string> AuthorizationTypes = new List<string>();
        bool isAuthenticated = false;
        string _handleunauthorized = master.framework.Configuration.MasterFramework.Authorization.URLUnauthorized;
        string _handlenotauthenticated = master.framework.Configuration.MasterFramework.Authentication.URLUnauthenticated;
        bool AcceptAnonymous = false;

        public CheckAuthorizeAttribute(bool acceptAnonymous, string urlHandleUnauthorized = null, params string[] types)
        {
            if (!string.IsNullOrWhiteSpace(urlHandleUnauthorized))
            {
                _handleunauthorized = urlHandleUnauthorized;
            }
            AuthorizationTypes.AddRange(types.ToList());
            AcceptAnonymous = acceptAnonymous;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (AcceptAnonymous) { return true; }
            isAuthenticated = httpContext.User.Identity.IsAuthenticated;
            bool ret = isAuthenticated;

            
            master.framework.dto.UserAuth userAuth = null;
            userAuth = master.framework.web.Extensions.GetObjectFromSession<master.framework.dto.UserAuth>(master.framework.Configuration.MasterFramework.Authentication.UserSessionName);

            if (userAuth != null)
            {
                if (AuthorizationTypes.Count == 0 && !AcceptAnonymous) { return true; }
                if (userAuth.Actions.Any(o => o.ToLower() == master.framework.Configuration.MasterFramework.Authorization.Action.ADM)) { return true; }

                foreach (var item in AuthorizationTypes)
                {
                    ret = (userAuth.Actions.Any(o => o == item));
                    if (ret) { break; }
                }
            }

            return ret;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.StatusDescription = "Unauthorized access.";
                filterContext.HttpContext.Response.Write("401, Unauthorized access.");
                filterContext.Result = new EmptyResult();
                filterContext.HttpContext.Response.End();       
            }
            else
            {
                filterContext.Result = new RedirectResult(isAuthenticated ? _handleunauthorized : _handlenotauthenticated);
                //base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
