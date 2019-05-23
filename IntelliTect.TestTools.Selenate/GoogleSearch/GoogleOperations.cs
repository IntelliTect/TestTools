using System;
using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;
using IntelliTect.TestTools.SelenateExtensions;
using IntelliTect.TestTools.Selenate;

namespace GoogleSearch
{
    public class GoogleOperations
    {
        public GoogleOperations(GoogleBrowser browser)
        {
            Browser = browser;
            Harness = new GoogleHarness(Browser);
            Element = new Element(Browser.Driver);
        }

        public bool SearchForItem(string searchItem)
        {
            Browser.Driver.Navigate().GoToUrl(Harness.URL);
            Harness.SearchInput.SendKeysWhenReady(Browser.Driver, searchItem);
            Harness.SearchInput.SendKeys(Keys.Return);
            return Browser.WaitUntil(() => Harness.SearchResultsDiv.Displayed);
        }

        public bool FindSearchResultItem(string result)
        {
            // Don't need to await this since it would just be on one line
            var headers = Harness.SearchResultsHeadersList;
            return Browser.WaitUntil(() => headers.Any(h => h.Text == result), 5);
        }

        public bool GoToHomePage()
        {
            Element.ClickElementWhenReady(Harness.GoHomeButton);
            return Browser.WaitUntil(() => Harness.GoogleSearchButton.Displayed);
        }

        private GoogleBrowser Browser { get; }
        private GoogleHarness Harness { get; }
        private Element Element { get; }
    }
}
