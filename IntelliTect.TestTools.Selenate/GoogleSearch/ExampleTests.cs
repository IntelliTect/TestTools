using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntelliTect.TestTools.Selenate;

namespace GoogleSearch
{
    [TestClass]
    public class ExampleTests
    {
        [TestInitialize]
        public void Setup()
        {
            Browser = new GoogleBrowser(BrowserType.Chrome);
            Google = new GoogleFunctions(Browser);
            Harness = new GoogleHarness(Browser);
        }

        [TestMethod]
        public async Task SearchForSeleniumOnGoogle()
        {
            Assert.IsTrue(await Google.SearchForItem("selenium automation"), 
                "No search results displayed when they were expected");
            Assert.IsTrue(await Google.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Did not find a specific search result for Selenium - Web Browser Automation");
        }

        [TestMethod]
        public async Task VerifySeleniumDoesNotExistForElement()
        {
            await Google.SearchForItem("selenium element");
            Assert.IsFalse(await Google.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Found a specific search result for Selenium - Web Browser Automation when none was expected");
        }

        [TestMethod]
        public async Task ReturnToHomepage()
        {
            await Google.SearchForItem("selenium automation");
            await Google.GoToHomePage();
            Assert.IsFalse(await Browser.WaitFor(() => Harness.SearchResultsDiv.Displayed),
                "Search results displayed when they were not expected");
        }

        [TestCleanup]
        public void Teardown()
        {
            Browser.TakeScreenshot();
            Browser.Driver.Quit();
        }

        private GoogleBrowser Browser { get; set; }
        private GoogleFunctions Google { get; set; }
        private GoogleHarness Harness { get; set; }
    }
}
