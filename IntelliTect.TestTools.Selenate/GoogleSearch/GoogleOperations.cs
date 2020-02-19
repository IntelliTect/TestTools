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
            Element = new ElementHandler(Browser.Driver);
        }

        public bool SearchForItem(string searchItem)
        {
            Browser.Driver.Navigate().GoToUrl(Harness.URL);
            Element.WaitForEnabledState(Harness.SearchInput);
            Element.SendKeysWhenReady(Harness.SearchInput, searchItem);
            Harness.SearchInput.SendKeys(Keys.Return);
            return Element.WaitForVisibleState(Harness.SearchResultsDiv);
        }

        public bool FindSearchResultItem(string result)
        {
            var headers = Harness.SearchResultsHeadersList;
            return Browser.WaitUntil(() => headers.Any(h => h.Text == result), 5);
        }

        public bool GoToHomePage()
        {
            Element.ClickElementWhenReady(Harness.GoHomeButton);
            return Element.WaitForVisibleState(Harness.GoogleSearchButton);
        }

        private GoogleBrowser Browser { get; }
        private GoogleHarness Harness { get; }
        private ElementHandler Element { get; }
    }
}
