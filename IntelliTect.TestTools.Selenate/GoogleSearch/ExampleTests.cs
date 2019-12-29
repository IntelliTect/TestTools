using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntelliTect.TestTools.Selenate;
using System.IO;
using OpenQA.Selenium.Remote;

namespace GoogleSearch
{
    [TestClass]
    public class ExampleTests
    {
        [TestInitialize]
        public void Setup()
        {
            Browser = new GoogleBrowser(BrowserType.Chrome);
            Google = new GoogleOperations(Browser);
            Harness = new GoogleHarness(Browser);
        }

        [TestMethod]
        public void SearchForSeleniumOnGoogle()
        {
            Assert.IsTrue(Google.SearchForItem("selenium browser automation"), 
                "No search results displayed when they were expected");
            Assert.IsTrue(Google.FindSearchResultItem("SeleniumHQ Browser Automation"),
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
            // Two ways to take screenshots.
            Browser.TakeScreenshot();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "screenshot");
            string filename = $"{((RemoteWebDriver)Browser.Driver).Capabilities.BrowserName}_override_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png";
            Browser.TakeScreenshot(path, filename);
            Browser.Dispose();
        }

        private GoogleBrowser Browser { get; set; }
        private GoogleOperations Google { get; set; }
        private GoogleHarness Harness { get; set; }
    }
}
