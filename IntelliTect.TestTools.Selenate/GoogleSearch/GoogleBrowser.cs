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
            return Find(by).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        // Instead of doing this, I should just do a separate project demonstrating async usage on something more complex than a Google search
        public async Task<IWebElement> FindElementAsync(By by)
        {
            return await Find(by);
        }

        // When running tests in succession, Google sometimes refuses the connection.
        // Refresh to kick it into gear.
        private async Task<IWebElement> Find(By by)
        {
            try
            {
                return await base.FindElement(by);
            }
            catch (AggregateException)
            {
                Driver.Navigate().Refresh();
                return await base.FindElement(by);
            }
        }
    }
}
