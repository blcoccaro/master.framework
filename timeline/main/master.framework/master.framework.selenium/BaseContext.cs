using System;
using System.Net;

namespace master.framework.selenium
{
    public abstract class BaseContext : IDisposable
    {
        public abstract Uri GetURI(EnvironmentType env);
        public bool Initialized { get; set; }
        public OpenQA.Selenium.Remote.RemoteWebDriver Driver { get; set; }
        public EnvironmentType Environment { get; set; }
        private string browser;

        public NUnit.Framework.TestContext TestContext { get; set; }

        public int TimeoutDefaultSeconds { get; set; }

        public BaseContext(EnvironmentType environment)
        {
            Environment = environment;

        }

        public OpenQA.Selenium.Support.UI.WebDriverWait GetWait()
        {
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(this.Driver, TimeSpan.FromSeconds(TimeoutDefaultSeconds));
            return wait;
        }

        public void WaitUntilElement(string id, SearchType search)
        {
            var wait = GetWait();
            wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.ElementToBeSelected(this.Driver.GetElementById(id, search)));
        }
        public void WaitUntilElementClickable(string id, SearchType search)
        {
            var wait = GetWait();

            OpenQA.Selenium.IWebElement element = wait.Until(OpenQA.Selenium.Support.UI.ExpectedConditions.ElementToBeClickable(this.Driver.GetElementById(id, search)));
        }

        public void InitializeChrome(EnvironmentType environment)
        {
            if (Initialized)
            {
                return;
            }
            //OpenQA.Selenium.Chrome.ChromeOptions.Capability
            //OpenQA.Selenium.Remote.DesiredCapabilities capability = OpenQA.Selenium.Remote.DesiredCapabilities.Chrome();
            Uri server = null;
            Initialized = true;
            server = GetURI(environment);
            //this.driver = new RemoteWebDriver(server, capability);
            this.Driver = new OpenQA.Selenium.Chrome.ChromeDriver();
            Driver.Manage().Window.Maximize();

            Driver.Navigate().GoToUrl(GetURI(environment));
            if (checkApplicationRunning(Driver.Url))
            {
                throw new NotAnswerException(Driver.Url);
            }
        }

        public void Dispose()
        {
            try
            {
                this.Driver.Quit();
                this.Driver.Dispose();
                this.Driver = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error: {0}", ex.Message));
            }
        }

        public static Boolean checkApplicationRunning(String URL)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URL);
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;
            }
            catch
            {
                exists = false;
            }
            return exists;
        }
    }
}
