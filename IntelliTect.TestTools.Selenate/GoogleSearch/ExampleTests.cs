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
        public void SearchForSeleniumOnGoogle()
        {
            Assert.IsTrue(Google.SearchForItem("selenium automation"), 
                "No search results displayed when they were expected");
            Assert.IsTrue(Google.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Did not find a specific search result for Selenium - Web Browser Automation");
        }

        [TestMethod]
        public void VerifySeleniumDoesNotExistForElement()
        {
            Google.SearchForItem("selenium element");
            Assert.IsFalse(Google.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Found a specific search result for Selenium - Web Browser Automation when none was expected");
        }

        [TestMethod]
        public void ReturnToHomepage()
        {
            Google.SearchForItem("selenium automation");
            Google.GoToHomePage();
            // Different examples for how to verify an element is not displayed or present in the DOM
            // Note: Invoking this like: Browser.WaitFor(() => !Harness.SearchResultsDiv.Displayed)) produces incosistent results due to how elements not being present are handled
            Assert.IsFalse(Browser.WaitFor(() => Harness.SearchResultsDiv.Displayed).GetAwaiter().GetResult(),
                "Search results displayed when they were not expected");
            Assert.IsTrue(!Browser.WaitFor(() => Harness.SearchResultsDiv.Displayed).GetAwaiter().GetResult());
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
