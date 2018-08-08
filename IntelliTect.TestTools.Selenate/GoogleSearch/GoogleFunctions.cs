using OpenQA.Selenium;
using System.Linq;
using IntelliTect.TestTools.SelenateExtensions;

namespace GoogleSearch
{
    class GoogleFunctions
    {
        public GoogleFunctions(GoogleBrowser browser)
        {
            Browser = browser;
            Harness = new GoogleHarness(Browser);
        }

        public bool SearchForItem(string searchItem)
        {
            Browser.Driver.Navigate().GoToUrl(Harness.URL);
            Harness.SearchInput.FillInWith(searchItem);
            Harness.SearchInput.SendKeys(Keys.Return);
            return Browser.WaitFor(() => Harness.SearchResultsDiv.Displayed);
        }

        public bool FindSearchResultItem(string result)
        {
            var headers = Harness.SearchResultsHeadersList;
            return Browser.WaitFor(() => headers.Any(h => h.Text == result));
        }

        public bool GoToHomePage()
        {
            Harness.GoHomeButton.Click();
            return Browser.WaitFor(() => Harness.GoogleSearchButton.Displayed);
        }

        private GoogleBrowser Browser { get; }
        private GoogleHarness Harness { get; }
    }
}
