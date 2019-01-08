using OpenQA.Selenium;
using IntelliTect.TestTools.Selenate;
using System;
using System.Threading.Tasks;

namespace GoogleSearch
{
    public class GoogleBrowser : Browser
    {
        public GoogleBrowser(BrowserType browser) : base(browser)
        {
        }

        public IWebElement FindElement(By by)
        {
            return Find(by);
        }

        // When running tests in succession, Google sometimes refuses the connection.
        // Refresh to kick it into gear.
        private IWebElement Find(By by)
        {
            try
            {
                return base.FindElement(by);
            }
            catch (AggregateException)
            {
                Driver.Navigate().Refresh();
                return base.FindElement(by);
            }
        }
    }
}
