using System;

namespace master.framework
{
    public static class Configuration
    {
        public static class MasterFramework
        {
            public static class Database
            {
                public static class Historical
                {
                    public static bool isOFF
                    {
                        get
                        {
                            bool ret = false;
                            try
                            {
                                string aux = System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Database::Historical::isOFF"];
                                aux = aux ?? string.Empty;
                                if (aux.ToLower() == "true")
                                {
                                    ret = true;
                                }
                            }
                            catch (Exception)
                            {
                            }
                            return ret;
                        }
                    }
                }
            }
            public static class Service
            {
                public static class Debug
                {
                    public static bool isON
                    {
                        get
                        {
                            bool ret = false;
                            try
                            {
                                string aux = System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Service::Debug::isON"];
                                aux = aux ?? string.Empty;
                                if (aux.ToLower() == "true")
                                {
                                    ret = true;
                                }
                            }
                            catch (Exception)
                            {
                            }
                            return ret;
                        }
                    }
                }
                public static int TimerInMilliseconds
                {
                    get
                    {
                        int ret = 0;
                        ret = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Service::TimerInMilliseconds"]);
                        return ret;
                    }
                }
            }
            public static class Email
            {
                public static string Login
                {
                    get { return System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Email::Login"]; }
                }
                public static string Password
                {
                    get { return System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Email::Password"]; }
                }
                public static string SMTP
                {
                    get { return System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Email::SMTP"]; }
                }
                public static int Port
                {
                    get
                    {
                        int ret = 0;
                        ret = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Email::Port"]);
                        return ret;
                    }
                }
                public static bool EnableSSL
                {
                    get
                    {
                        bool ret = false;
                        try
                        {
                            string aux = System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Email::EnableSsl"];
                            aux = aux ?? string.Empty;
                            if (aux.ToLower() == "true")
                            {
                                ret = true;
                            }
                        }
                        catch (Exception)
                        {
                        }
                        return ret;
                    }
                }
            }
            public static class Authentication
            {
                public const string UserSessionName = "__mf.session.ua__";
                public static int MinutesToCheckSession
                {
                    get
                    {
                        int ret = 20;
                        ret = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Authentication::MinutesToCheckSession"]);
                        return ret;
                    }
                }
                public static string URLUnauthenticated
                {
                    get { return System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Authentication::URLUnauthenticated"]; }
                }

            }
            public static class Authorization
            {
                public static string URLUnauthorized
                {
                    get { return System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Authorization::URLUnauthorized"]; }
                }
                public class Action
                {
                    public static string ADM
                    {
                        get
                        {
                            string ret = System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::Authorization::Action::ADM"];
                            if (string.IsNullOrWhiteSpace(ret))
                            {
                                ret = "MF_ROOT";
                            }
                            return ret;
                        }
                    }
                }
            }
            public static class WCF
            {
                /// <summary>
                /// MF::CustomHeaderName
                /// Default: FCHN
                /// </summary>
                public static string CustomHeaderName
                {
                    get
                    {
                        string ret = System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::WCF::CustomHeaderName"];
                        if (string.IsNullOrWhiteSpace(ret))
                        {
                            ret = "FCHN";
                        }
                        return ret;
                    }
                }
                /// <summary>
                /// MF::CustomHeaderNamespace
                /// Default: FCHNS
                /// </summary>
                public static string CustomHeaderNamespace
                {
                    get
                    {
                        string ret = System.Configuration.ConfigurationManager.AppSettings["Configuration::MasterFramework::WCF::CustomHeaderNamespace"];
                        if (string.IsNullOrWhiteSpace(ret))
                        {
                            ret = "FCHNS";
                        }
                        return ret;
                    }
                }
            }
        }

    }
}
