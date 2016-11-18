using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace master.framework.web
{
    public class BaseController : Controller
    {
        #region Constants
        private const string _COOKIE_LASTHOST = "mf_lasthost";
        private const string _COOKIE_LASTCHECK = "mf_lastcheckdate";
        #endregion

        #region Overrides
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            this.logError("Error.", ex);
            ViewResult result = new ViewResult() { ViewName = "Error" };
            result.ViewData.Add("msg", ex.ToDataLogging().Message);
            filterContext.ExceptionHandled = true;
            filterContext.Result = result;
            base.OnException(filterContext);
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            SetLastHost(requestContext.HttpContext);
            base.Initialize(requestContext);
        }
        #endregion

        #region User
        public bool isLogged
        {
            get { return false; }
        }


        public master.framework.dto.UserAuth UserAuth
        {
            get
            {
                return master.framework.web.Extensions.GetObjectFromSession<master.framework.dto.UserAuth>(master.framework.Configuration.MasterFramework.Authentication.UserSessionName);
            }
            set
            {
                master.framework.web.Extensions.SetObjectToSession(master.framework.Configuration.MasterFramework.Authentication.UserSessionName, value);
            }
        }

        #region Obsoletes Methods
        [Obsolete("Property is obsolete, instead use UserAuth")]
        public System.Security.Principal.IPrincipal User
        {
            get
            {
                return base.User;
            }
        }
        #endregion
        #endregion

        #region Last Host
        private void SetLastHost(System.Web.HttpContextBase httpContext)
        {

            string host = string.Empty;
            if (httpContext != null && httpContext.Request != null && httpContext.Request.Url != null)
            {
                host = System.Web.HttpContext.Current.Request.Url.Authority;
            }
            string save = string.Format("{0}", host);
            master.framework.web.Extensions.AddCookie(_COOKIE_LASTHOST, value: save, httpContext: httpContext);

        }
        #endregion

        #region Check If Must Check Session
        public bool MustCheckSession()
        {
            bool ret = true;

            try
            {
                string lastTimeString = master.framework.web.Extensions.RequestCookie<string>(_COOKIE_LASTCHECK);
                if (lastTimeString != null)
                {
                    string host = string.Empty;
                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Url != null)
                    {
                        host = System.Web.HttpContext.Current.Request.Url.Authority;
                    }
                    var hostCookie = master.framework.web.Extensions.RequestCookie<string>(_COOKIE_LASTHOST);
                    if (hostCookie == null) { hostCookie = string.Empty; }
                    if (host.ToLower() == hostCookie.ToLower())
                    {
                        long lastTime = 0;
                        if (long.TryParse(lastTimeString, out lastTime))
                        {
                            DateTime newDate = new DateTime(lastTime);
                            ret = newDate <= DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return ret;
        }
        public void RemoveCheckSessionCookie()
        {
            master.framework.web.Extensions.RemoveCookie(_COOKIE_LASTCHECK);
        }
        public void SetCheckSessionCookie()
        {

            string lastTimeString = master.framework.web.Extensions.RequestCookie<string>(_COOKIE_LASTCHECK);
            if (lastTimeString != null)
            {
                RemoveCheckSessionCookie();
            }
            var dateAux = DateTime.Now.AddMinutes(master.framework.Configuration.MasterFramework.Authentication.MinutesToCheckSession);
            string save = string.Format("{0}", dateAux.Ticks);
            master.framework.web.Extensions.AddCookie(_COOKIE_LASTCHECK, value: save);

        }

        #endregion

        #region Authorization Check
        public bool IsAuthorizedTo(string code)
        {
            bool ret = false;

            if (!string.IsNullOrWhiteSpace(code))
            {
                code = code.ToLower();
                if (UserAuth != null)
                {
                    List<string> authorization = new List<string>();
                    if (UserAuth.Actions.Any(o => o.ToLower() == master.framework.Configuration.MasterFramework.Authorization.Action.ADM)) { return true; }
                    ret = UserAuth.Actions.Any(o => o == code);
                }
            }

            return ret;
        }
        #endregion
    }
}
