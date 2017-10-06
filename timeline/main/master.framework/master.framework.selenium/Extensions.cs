using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Reflection;

namespace master.framework.selenium
{
    public static class Extensions
    {
        /// <summary>
        /// How Many Try before throw error
        /// </summary>
        public static int HowManyTry = 30;

        #region Extensions for RemoteWebDrive - Alert
        /// <summary>
        /// Alert is present?
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        public static bool HasAlert(this RemoteWebDriver drive)
        {
            try
            {
                drive.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        /// <summary>
        /// Get Message from the Alert
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        public static string AlertMessage(this RemoteWebDriver drive)
        {
            if (drive.HasAlert())
            {
                var alert = drive.SwitchTo().Alert();

                return alert.Text;

            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Click on OK in the Alert
        /// </summary>
        /// <param name="drive"></param>
        public static void AlertMessageClick(this RemoteWebDriver drive)
        {
            if (drive.HasAlert())
            {
                var alert = drive.SwitchTo().Alert();
                alert.Accept();
            }
        }
        #endregion

        #region Extensions for RemoteWebDrive - GetElementByIdAnd... Execute an Action
        /// <summary>
        /// getElementById - Return Select
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of Element</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        /// <returns>Select object</returns>
        public static SelectElement GetElementByIdReturnSelect(this RemoteWebDriver drive, string id, SearchType search = SearchType.Exact, string tagToGet = null)
        {
            int total = 0;
            bool cont = true;
            do
            {
                total++;
                cont = false;
                try
                {
                    var element = drive.GetElementById(id, search: search, tagToGet: tagToGet);
                    var select = new SelectElement(element);
                    var ok = select.SelectedOption;
                    return select;
                }
                catch (Exception ex)
                {
                    cont = true;
                    System.Threading.Thread.Sleep(1);
                    if (total == HowManyTry) { throw; }
                }
            } while (cont);
            return null;
        }
        /// <summary>
        /// getElementById and Click
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of Element</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        public static void GetElementByIdAndClick(this RemoteWebDriver drive, string id, SearchType search = SearchType.Exact, string tagToGet = null)
        {
            int total = 0;
            bool cont = true;
            do
            {
                total++;
                cont = false;
                try
                {
                    var element = drive.GetElementById(id, search: search, tagToGet: tagToGet);
                    element.Click();
                }
                catch (Exception ex)
                {
                    cont = true;
                    System.Threading.Thread.Sleep(1);
                    if (total == HowManyTry) { throw; }
                }
            } while (cont);
        }
        /// <summary>
        /// getElementById and set value to element
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of Element</param>
        /// <param name="value">Value to Set</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        public static void GetElementByIdAndSetValue(this RemoteWebDriver drive, string id, string value, SearchType search = SearchType.Exact, string tagToGet = null)
        {
            int total = 0;
            bool cont = true;
            do
            {
                total++;
                cont = false;
                try
                {
                    var element = drive.GetElementById(id, search: search, tagToGet: tagToGet);
                    element.Clear();
                    element.SendKeys(value);
                }
                catch (Exception ex)
                {
                    cont = true;
                    System.Threading.Thread.Sleep(1);
                    if (total == HowManyTry) { throw; }
                }
            } while (cont);
        }
        /// <summary>
        /// getDropDownListById
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of Element</param>
        /// <param name="value">Value to Set</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        public static void DropDownListSelectValue(this RemoteWebDriver drive, string id, string value, SearchType search = SearchType.Exact, string tagToGet = null)
        {
            SelectElement select = new SelectElement(drive.GetElementById(id, search, tagToGet: tagToGet));

            select.SelectByValue(value);
        }
        #endregion

        #region Extensions for RemoteWebDrive - GetElement
        /// <summary>
        /// getElementBy
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="type">GetElementType</param>
        /// <param name="key">Key</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        /// <param name="extraSearch">Extra search</param>
        /// <returns></returns>
        public static IList<IWebElement> GetListElementBy(this RemoteWebDriver drive, GetElementType type, string key, SearchType search = SearchType.Exact, string tagToGet = null, string extraSearch = null)
        {
            List<IWebElement> ret = null;
            int total = 0;
            bool cont = true;
            extraSearch = string.IsNullOrWhiteSpace(extraSearch) ? "" : " " + extraSearch;

            do
            {
                total++;
                cont = false;
                try
                {
                    string searchTag = string.Empty;
                    string searchCssSelector = string.Empty;

                    switch (type)
                    {
                        case GetElementType.Id:
                            searchTag = "id";
                            break;
                        case GetElementType.Css:
                            searchTag = "class";
                            break;
                    }

                    switch (search)
                    {
                        case SearchType.Exact:
                            searchCssSelector = string.Format("[{0}={1}]{2}", searchTag, key, extraSearch);
                            break;
                        case SearchType.AtEnd:
                            searchCssSelector = string.Format("[{0}$={1}]{2}", searchTag, key, extraSearch);
                            break;
                        case SearchType.AtStart:
                            searchCssSelector = string.Format("[{0}^={1}]{2}", searchTag, key, extraSearch);
                            break;
                        case SearchType.Contains:
                            searchCssSelector = string.Format("[{0}*={1}]{2}", searchTag, key, extraSearch);
                            break;
                    }

                    ret = drive.FindElementsByCssSelector(searchCssSelector).ToList();

                    if (!string.IsNullOrWhiteSpace(tagToGet))
                    {
                        ret = ret.Where(o => o.TagName == tagToGet).ToList();
                    }
                }
                catch (Exception ex)
                {
                    cont = true;
                    System.Threading.Thread.Sleep(1);
                    if (total == HowManyTry) { throw; }
                }
            } while (cont);
            return ret;
        }
        /// <summary>
        /// getElementBy
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="type">GetElementType</param>
        /// <param name="key">Key</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        /// <param name="extraSearch">Extra search</param>
        /// <returns></returns>
        public static IWebElement GetElementBy(this RemoteWebDriver drive, GetElementType type, string key, SearchType search = SearchType.Exact, string tagToGet = null, string extraSearch = null)
        {
            IWebElement ret = null;
            int total = 0;
            bool cont = true;

            do
            {
                total++;
                cont = false;
                try
                {
                    var aux = drive.GetListElementBy(type, key, search: search, tagToGet: tagToGet, extraSearch: extraSearch);
                    if (aux != null && aux.Count > 0)
                    {
                        ret = aux.FirstOrDefault();
                    }

                    if (ret != null)
                    {
                        var displayed = ret.Displayed;
                    }
                }
                catch (Exception ex)
                {
                    cont = true;
                    System.Threading.Thread.Sleep(1);
                    if (total == HowManyTry) { throw; }
                }
            } while (cont);
            return ret;
        }
        /// <summary>
        /// GetElementById
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of Element</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        /// <param name="extraSearch">Extra Search</param>
        /// <returns></returns>
        public static IWebElement GetElementById(this RemoteWebDriver drive, string id, SearchType search = SearchType.Exact, string tagToGet = null, string extraSearch = null)
        {
            return drive.GetElementBy(type: GetElementType.Id, key: id, search: search, tagToGet: tagToGet);
        }
        /// <summary>
        /// getElementByClass
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="cssClass">CSS</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        /// <param name="extraSearch"></param>
        /// <returns></returns>
        public static IWebElement GetElementByClass(this RemoteWebDriver drive, string cssClass, SearchType search = SearchType.Exact, string tagToGet = null, string extraSearch = null)
        {
            return drive.GetElementBy(type: GetElementType.Css, key: cssClass, search: search, tagToGet: tagToGet, extraSearch: extraSearch);
        }
        #endregion

        #region Extensions for RemoteWebDrive - Mouse actions
        /// <summary>
        /// GetElementAndSetMouseOver
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of Element</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        public static void ElementMouseOver(this RemoteWebDriver drive, string id, SearchType search = SearchType.Exact, string tagToGet = null)
        {
            var element = drive.GetElementById(id, search: search, tagToGet: tagToGet);
            Actions actions = new Actions(drive);
            actions.MoveToElement(element);
            actions.Build().Perform();
        }
        #endregion

        #region Extensions for RemoteWebDrive - ElementExist
        /// <summary>
        /// Check if element exist
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="id">Id of element</param>
        /// <param name="checkDisplayed">Check if Element has Displayed = false</param>
        /// <param name="search">Type of search for the Id</param>
        /// <param name="tagToGet">Tag name for the search</param>
        /// <returns></returns>
        public static bool ElementExists(this RemoteWebDriver drive, string id, bool checkDisplayed = false, SearchType search = SearchType.Exact, string tagToGet = null)
        {
            try
            {
                var element = drive.GetElementById(id, search: search, tagToGet: tagToGet);
                if (element == null) return false;
                var displayed = element.Displayed;
                if (checkDisplayed && !displayed) { return false; }
                return true;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        #endregion

        #region Extensions for TechTalk.SpecFlow.ScenarioContext
        /// <summary>
        /// Set Value in SpecFlow Context
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj"></param>
        /// <param name="key">Key</param>
        /// <param name="value">Object</param>
        public static void SetValueFromContext<T>(this TechTalk.SpecFlow.ScenarioContext obj, string key, T value)
        {
            if (obj.ContainsKey(key))
            {
                obj.Remove(key);
            }
            obj.Set<T>(value, key);
        }
        /// <summary>
        /// Get Value in SpecFlow Context
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj"></param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static T GetValueFromContext<T>(this TechTalk.SpecFlow.ScenarioContext obj, string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj.Get<T>(key);
            }
            else
            {
                return default(T);
            }
        }
        #endregion
    }
}
