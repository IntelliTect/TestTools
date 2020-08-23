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
        private GoogleBrowser Browser { get; set; }
        private GoogleOperations Google { get; set; }
        //private GoogleHarness Harness { get; set; }
        private ElementHandler Element { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Browser = new GoogleBrowser(BrowserType.Chrome);
            Google = new GoogleOperations(Browser);
            Element = new ElementHandler(Browser.Driver);
            //Harness = new GoogleHarness();
        }

        

        [TestMethod]
        public void SearchForSeleniumOnGoogle()
        {
            Google.NavigateToGoogle();
            Assert.IsTrue(Google.SearchForItem("selenium browser automation"), 
                "No search results displayed when they were expected");
            Assert.IsTrue(Google.FindSearchResultItem("SeleniumHQ Browser Automation"),
                "Did not find a specific search result for Selenium - Web Browser Automation");
        }

        [TestMethod]
        public void VerifySeleniumDoesNotExistForElement()
        {
            Google.NavigateToGoogle();
            Google.SearchForItem("selenium element");
            Assert.IsFalse(Google.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Found a specific search result for Selenium - Web Browser Automation when none was expected");
        }

        [TestMethod]
        public void ReturnToHomepage()
        {
            Google.NavigateToGoogle();
            Google.SearchForItem("selenium automation");
            Google.GoToHomePage();
            Assert.IsTrue(Element.WaitForInvisibleState(GoogleHarness.SearchResultsDiv),
                "Search results displayed when they were not expected");
        }

        [TestCleanup]
        public void Teardown()
        {
            // Two ways to take screenshots.
            Browser.TakeScreenshot();
            FileInfo file = new FileInfo(
                Path.Combine(Directory.GetCurrentDirectory(), 
                "screenshot", 
                $"{((RemoteWebDriver)Browser.Driver).Capabilities.BrowserName}_override_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png"));
            Directory.CreateDirectory(file.DirectoryName);
            Browser.TakeScreenshot(file);
            Browser.Dispose();
        }
    }
}
