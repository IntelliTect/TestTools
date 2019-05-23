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
            Element = new Element(Browser.Driver);
            Google = new GoogleOperations(Browser);
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
            Assert.IsFalse(Browser.WaitUntil(() => Harness.SearchResultsDiv.Displayed),
                "Search results displayed when they were not expected");
        }

        [TestCleanup]
        public void Teardown()
        {
            Browser.TakeScreenshot();
            Browser.Driver.Quit();
        }

        private GoogleBrowser Browser { get; set; }
        private Element Element { get; set; }
        private GoogleOperations Google { get; set; }
        private GoogleHarness Harness { get; set; }
    }
}
