using OpenQA.Selenium;
using System.Linq;
using IntelliTect.TestTools.Selenate;
using System;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace GoogleSearch
{
    public class GoogleOperations
    {
        public GoogleOperations(IWebDriver driver)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        private GooglePage Google => new GooglePage(Driver);
        private IWebDriver Driver { get; }
        private ElementHandler Page { get; }

        public bool NavigateToGoogle()
        {
            //Driver.Navigate().GoToUrl(FrontPage.URL);
            Driver.Url = FrontPage.URL.AbsoluteUri;
            return Google.FrontPage.SearchInput.WaitForEnabledState();
            //return Page.WaitForEnabledState(FrontPage.SearchInput);
        }

        public bool SearchForItem(string searchItem)
        {
            Google.FrontPage.SearchInput.SendKeysWhenReady(searchItem);
            Google.FrontPage.SearchInput.SendKeysWhenReady(Keys.Return);
            return Google.ResultsPage.SearchResultsDiv.WaitForVisibleState();
            //Page.SendKeysWhenReady(FrontPage.SearchInput, searchItem);
            //Page.SendKeysWhenReady(FrontPage.SearchInput, Keys.Return);
            //return Page.WaitForVisibleState(ResultsPage.SearchResultsDiv);
        }

        public bool FindSearchResultItem(string result)
        {
            return Google.ResultsPage.SearchResultsHeadersList.ContainsText(result);

            // Use custom wait due to needing a Any() call on the FindElements result.
            //var wait = new DefaultWait<IWebDriver>(Driver);
            //wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            //wait.Timeout = TimeSpan.FromSeconds(5);
            //wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            //try
            //{
            //    return wait.Until(d => d.FindElements(ResultsPage.ResultsHeaderBy).Any(h => h.Text == result));
            //}
            //catch(WebDriverTimeoutException)
            //{
            //    return false;
            //}
        }

        public bool GoToHomePage()
        {
            Google.ResultsPage.GoHomeButton.ClickWhenReady();
            return Google.FrontPage.SeachButton.WaitForEnabledState();

            //var homeButton = new ElementHandler(Driver.Driver, By.CssSelector(""));

            //homeButton.Timeout(TimeSpan.FromSeconds(5)).Click();

            //Actions action = new Actions(Driver.Driver);
            //action
            //    .Click()
            //    .SendKeys("some text");

            //Google
            //    .SearchButton
            //    .PerformAction(action);


            //Page
            //    .Element(ResultsPage.GoHomeButton)
            //    .SetTimeoutSeconds(5)
            //    .ClickElementWhenReady()
            //    .TakeScreenshot();
            ////Element.ClickElementWhenReady(GoogleSearchResultsPage.GoHomeButton);
            //return Page.WaitForVisibleState(FrontPage.GoogleSearchButton);
        }
    }
}
