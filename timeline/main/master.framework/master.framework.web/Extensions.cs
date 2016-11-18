using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.web
{
    public static class Extensions
    {
        /// <summary>
        /// Cast para o Controller correto
        /// </summary>
        /// <typeparam name="T">Tipo do Controller</typeparam>
        /// <param name="obj">ViewContext</param>
        /// <returns>Controler com o Objeto esperado</returns>
        public static T Controller<T>(this System.Web.Mvc.ViewContext obj) where T : System.Web.Mvc.Controller
        {
            return obj.Controller as T;
        }
        public static T GetObjectFromSession<T>(string name, System.Web.HttpContextBase httpContext = null) where T : class
        {
            if (httpContext!=null)
            {
                return httpContext.Session[name] as T;
            }
            else if (System.Web.HttpContext.Current != null)
            {
                return System.Web.HttpContext.Current.Session[name] as T;
            }
            return null;
        }
        public static void SetObjectToSession(string name, object obj, System.Web.HttpContextBase httpContext = null)
        {
            if (httpContext != null)
            {
                httpContext.Session[name] = obj;
            }
            else if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Session[name] = obj;
            }
        }

        public static T RequestCookie<T>(string name, System.Web.HttpContextBase httpContext = null) where T : class
        {
            if (httpContext != null)
            {
                var cookie = httpContext.Request.Cookies[name];
                if (cookie != null)
                {
                    return cookie.Value as T;
                }
            }
            else if (System.Web.HttpContext.Current != null)
            {
                var cookie = System.Web.HttpContext.Current.Request.Cookies[name];
                if (cookie != null)
                {
                    return cookie.Value as T;
                }
            }
            return null;
        }
        public static void AddCookie(string name, string value = "", object obj = null, System.Web.HttpContextBase httpContext = null)
        {
            System.Web.HttpCookie cookie = new System.Web.HttpCookie(name);
            cookie.Value = value;
            cookie.Expires = DateTime.Now.AddDays(1);
            if (httpContext != null)
            {
                httpContext.Response.Cookies.Add(cookie);
            }
            else if (System.Web.HttpContext.Current != null)
            {
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        public static void RemoveCookie(string name, System.Web.HttpContextBase httpContext = null)
        {
            if (httpContext != null)
            {
                if (httpContext.Request.Cookies.AllKeys.Contains(name))
                {
                    System.Web.HttpCookie cookie = httpContext.Request.Cookies[name];
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    httpContext.Response.Cookies.Add(cookie);
                }
            }
            else if (System.Web.HttpContext.Current != null)
            {
                if (System.Web.HttpContext.Current.Request.Cookies.AllKeys.Contains(name))
                {
                    System.Web.HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies[name];
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }


        public static string GetClientIpAddress(System.Web.HttpRequestBase request)
        {
            try
            {
                var userHostAddress = request.UserHostAddress;

                // Attempt to parse.  If it fails, we catch below and return "0.0.0.0"
                // Could use TryParse instead, but I wanted to catch all exceptions
                System.Net.IPAddress.Parse(userHostAddress);

                var xForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(xForwardedFor))
                    return userHostAddress;

                // Get a list of public ip addresses in the X_FORWARDED_FOR variable
                var publicForwardingIps = xForwardedFor.Split(',').Where(ip => !IsPrivateIpAddress(ip)).ToList();

                // If we found any, return the last one, otherwise return the user host address
                return publicForwardingIps.Any() ? publicForwardingIps.Last() : userHostAddress;
            }
            catch (Exception)
            {
                // Always return all zeroes for any failure (my calling code expects it)
                return "0.0.0.0";
            }
        }
        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = System.Net.IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }
    }
}

