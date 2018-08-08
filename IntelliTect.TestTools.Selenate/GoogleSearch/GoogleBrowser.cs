using OpenQA.Selenium;
using IntelliTect.TestTools.Selenate;

namespace GoogleSearch
{
    class GoogleBrowser : Browser
    {
        public GoogleBrowser(BrowserType browser) : base(browser)
        {
        }

        // When running tests in succession, Google sometimes refuses the connection.
        // Refresh to kick it into gear.
        public WebElement FindElement(By by)
        {
            WebElement result = base.FindElement(by);
            if(!result.Initialized)
            {
                Driver.Navigate().Refresh();
            }
            return result;
        }
    }
}
