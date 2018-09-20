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

        // When running tests in succession, Google sometimes refuses the connection.
        // Refresh to kick it into gear.
        public IWebElement FindElement(By by)
        {
            IWebElement result = null;
            try
            {
                result = base.FindElement(by).GetAwaiter().GetResult();
            }
            catch(AggregateException)
            {
                Driver.Navigate().Refresh();
                result = base.FindElement(by).GetAwaiter().GetResult();
            }
            return result;
            //result = base.FindElement(by);
            //if(!result.Initialized)
            //{
            //    Driver.Navigate().Refresh();
            //}
            //return result;
        }
    }
}
