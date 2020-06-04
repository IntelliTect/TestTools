using OpenQA.Selenium;
using System.Linq;
using IntelliTect.TestTools.Selenate;
using System;

namespace GoogleSearch
{
    public class GoogleOperations
    {
        public GoogleOperations(GoogleBrowser browser)
        {
            Browser = browser ?? throw new ArgumentNullException(nameof(browser));
            Harness = new GoogleHarness(Browser);
            Element = new ElementHandler(Browser.Driver);
        }

        public bool SearchForItem(string searchItem)
        {
            Browser.Driver.Navigate().GoToUrl(GoogleHarness.URL);
            Element.WaitForEnabledState(Harness.SearchInput);
            Element.SendKeysWhenReady(Harness.SearchInput, searchItem);
            Harness.SearchInput.SendKeys(Keys.Return);
            return Element.WaitForVisibleState(Harness.SearchResultsDiv);
        }

        public bool FindSearchResultItem(string result)
        {
            return Browser.WaitUntil(() => Harness.SearchResultsHeadersList.Any(h => h.Text == result), 5);
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
