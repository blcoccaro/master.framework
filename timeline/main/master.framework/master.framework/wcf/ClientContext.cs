using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.wcf
{
    /// <summary>
    /// This class will act as a custom context in the client side to hold the context information.
    /// </summary>
    public class ClientContext
    {
        public static void RepassInfo(string clientVersion = null, string system = null, string clientip = null, DateTime? clientdate = null, string iduser = null, string token = null, string userlogin = null, string websessionid = null)
        {
                master.framework.wcf.ClientContext.ClientVersion = string.IsNullOrWhiteSpace(clientVersion) ? master.framework.wcf.ClientInfo.ClientVersion : clientVersion;
                master.framework.wcf.ClientContext.System = string.IsNullOrWhiteSpace(system) ? master.framework.wcf.ClientInfo.System : system;
                master.framework.wcf.ClientContext.ClientIP = string.IsNullOrWhiteSpace(clientip) ? master.framework.wcf.ClientInfo.ClientIP : clientip;
                master.framework.wcf.ClientContext.ClientDate = clientdate == null ? master.framework.wcf.ClientInfo.ClientDate : clientdate;
                master.framework.wcf.ClientContext.IdUser = string.IsNullOrWhiteSpace(iduser) ? master.framework.wcf.ClientInfo.IdUser : iduser;
                master.framework.wcf.ClientContext.Token = string.IsNullOrWhiteSpace(token) ? master.framework.wcf.ClientInfo.Token : token;
                master.framework.wcf.ClientContext.UserLogin = string.IsNullOrWhiteSpace(userlogin) ? master.framework.wcf.ClientInfo.UserLogin : userlogin;
                master.framework.wcf.ClientContext.WebSessionId = string.IsNullOrWhiteSpace(websessionid) ? master.framework.wcf.ClientInfo.WebSessionId : websessionid;
        }
        public static string UserLogin { get; set; }
        public static string IdUser { get; set; }
        public static string System { get; set; }
        public static DateTime? ClientDate { get; set; }
        public static string WebSessionId { get; set; }
        public static string Token { get; set; }
        public static string ClientIP { get; set; }
        public static string ClientVersion { get; set; }
    }
}
