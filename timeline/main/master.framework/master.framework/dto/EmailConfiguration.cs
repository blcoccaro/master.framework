using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using master.framework;

namespace master.framework.dto
{
    public class EmailConfiguration
    {
        public string SMTP { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        
        public static EmailConfiguration InstanceDefault()
        {
            EmailConfiguration ret = null;

            if (!string.IsNullOrWhiteSpace(Configuration.MasterFramework.Email.SMTP))
            {
                try
                {
                    ret = new EmailConfiguration();
                    ret.EnableSSL = Configuration.MasterFramework.Email.EnableSSL;
                    ret.Login = Configuration.MasterFramework.Email.Login;
                    ret.Password = Configuration.MasterFramework.Email.Password;
                    ret.Port = Configuration.MasterFramework.Email.Port;
                    ret.SMTP = Configuration.MasterFramework.Email.SMTP;
                }
                catch (Exception ex)
                {
                }
            }

            return ret;
        }
    }
}
