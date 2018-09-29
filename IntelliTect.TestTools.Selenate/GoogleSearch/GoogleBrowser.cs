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
        
        public WebElement FindElement(By by)
        {
            return new WebElement(Find(by).ConfigureAwait(false).GetAwaiter().GetResult(), Driver);
        }

        // Instead of doing this, I should just do a separate project demonstrating async usage on something more complex than a Google search
        public async Task<WebElement> FindElementAsync(By by)
        {
            return new WebElement(await Find(by), Driver);
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
