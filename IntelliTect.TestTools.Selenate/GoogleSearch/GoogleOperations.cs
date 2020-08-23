using OpenQA.Selenium;
using System.Linq;
using IntelliTect.TestTools.Selenate;
using System;
using OpenQA.Selenium.Support.UI;

namespace GoogleSearch
{
    public class GoogleOperations
    {
        public GoogleOperations(GoogleBrowser browser)
        {
            Browser = browser ?? throw new ArgumentNullException(nameof(browser));
            Element = new ElementHandler(Browser.Driver);
        }

        public bool NavigateToGoogle()
        {
            Browser.Driver.Navigate().GoToUrl(GoogleHarness.URL);
            return Element.WaitForEnabledState(GoogleHarness.SearchInput);
        }

        public bool SearchForItem(string searchItem)
        {
            Element.SendKeysWhenReady(GoogleHarness.SearchInput, searchItem);
            Element.SendKeysWhenReady(GoogleHarness.SearchInput, Keys.Return);
            return Element.WaitForVisibleState(GoogleHarness.SearchResultsDiv);
        }

        public bool FindSearchResultItem(string result)
        {
            // Use custom wait due to needing a Any() call on the FindElements result.
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Browser.Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(5);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            try
            {
                return wait.Until(d => d.FindElements(GoogleHarness.SearchResultsHeadersList).Any(h => h.Text == result));
            }
            catch(WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool GoToHomePage()
        {
            Element.ClickElementWhenReady(GoogleHarness.GoHomeButton);
            return Element.WaitForVisibleState(GoogleHarness.GoogleSearchButton);
        }

        private GoogleBrowser Browser { get; }
        private ElementHandler Element { get; }
    }
}
