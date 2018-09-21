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
            return new WebElement(Find(by).ConfigureAwait(false).GetAwaiter().GetResult(), Driver);
        }

        public Task<IWebElement> FindElementAsync(By by)
        {
            return Find(by);
        }

        // When running tests in succession, Google sometimes refuses the connection.
        // Refresh to kick it into gear.
        private Task<IWebElement> Find(By by)
        {
            Task<IWebElement> result = null;
            try
            {
                result = base.FindElement(by);
            }
            catch (AggregateException)
            {
                Driver.Navigate().Refresh();
                result = base.FindElement(by);
            }
            return result;
        }
    }
}
