using System;
using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;
using IntelliTect.TestTools.SelenateExtensions;

namespace GoogleSearch
{
    public class GoogleFunctions
    {
        public GoogleFunctions(GoogleBrowser browser)
        {
            Browser = browser;
            Harness = new GoogleHarness(Browser);
        }

        public bool SearchForItem(string searchItem)
        {
            Browser.Driver.Navigate().GoToUrl(Harness.URL);
            Harness.SearchInput.FillInWithWhenReady(Browser.Driver, searchItem);
            Harness.SearchInput.SendKeys(Keys.Return);
            return Browser.WaitUntil(() => Harness.SearchResultsDiv.Displayed);
        }

        public bool FindSearchResultItem(string result)
        {
            // Don't need to await this since it would just be on one line
            var headers = Harness.SearchResultsHeadersList;
            return Browser.WaitUntil(() => headers.Any(h => h.Text == result));
        }

        public bool GoToHomePage()
        {
            Harness.GoHomeButton.ClickWhenReady(Browser.Driver);
            return Browser.WaitUntil(() => Harness.GoogleSearchButton.Displayed);
        }

        private GoogleBrowser Browser { get; }
        private GoogleHarness Harness { get; }
    }
}
