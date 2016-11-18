using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace master.framework.wcf
{
    public static class ClientInfo
    {
        public static string ClientIP
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.ClientIP;
                    }
                }
                return ret;
            }
        }
        public static DateTime? ClientDate
        {
            get
            {
                DateTime? ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.ClientDate;
                    }
                }
                return ret;
            }
        }
        public static string ClientVersion
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.ClientVersion;
                    }
                }
                return ret;
            }
        }
        
        public static string System
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.System;
                    }
                }
                return ret;
            }
        }
        public static string Token
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.Token;
                    }
                }
                return ret;
            }
        }
        public static string IdUser
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.IdUser;
                    }
                }
                return ret;
            }
        }
        public static string UserLogin
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.UserLogin;
                    }
                }
                return ret;
            }
        }
        public static string WebSessionId
        {
            get
            {
                string ret = null;
                if (OperationContext.Current != null)
                {
                    var obj = OperationContext.Current.IncomingMessageProperties["CurrentContext"] as master.framework.wcf.ServerContext;
                    if (obj != null)
                    {
                        ret = obj.WebSessionId;
                    }
                }
                return ret;
            }
        }

    }
}
