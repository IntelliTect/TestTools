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

        public async Task<bool> SearchForItem(string searchItem)
        {
            Browser.Driver.Navigate().GoToUrl(Harness.URL);
            await Harness.SearchInput.FillInWith(searchItem);
            await Harness.SearchInput.SendKeys(Keys.Return);
            return await Browser.WaitFor(() => Convert.ToBoolean(Harness.SearchResultsDiv.Displayed.Result));
        }

        public Task<bool> FindSearchResultItem(string result)
        {
            // Don't need to await this since it would just be on one line
            var headers = Harness.SearchResultsHeadersList;
            return Browser.WaitFor(() => headers.Any(h => h.Text.Result == result));
        }

        public async Task<bool> GoToHomePage()
        {
            await Harness.GoHomeButton.WaitAndClick();
            return await Browser.WaitFor(() => Convert.ToBoolean(Harness.GoogleSearchButton.Displayed.Result));
        }

        private GoogleBrowser Browser { get; }
        private GoogleHarness Harness { get; }
    }
}
