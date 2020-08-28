using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntelliTect.TestTools.Selenate;
using System.IO;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;

namespace GoogleSearch
{
    [TestClass]
    public class ExampleTests
    {
        //private DriverHandler Browser { get; set; }
        private GoogleOperations Test { get; set; }
        //private GoogleHarness Harness { get; set; }
        //private ElementHandler Element { get; set; }
        private IWebDriver Driver { get; set; }
        //private SeleniumHandler Selenium { get; set; }
        private GooglePage Google => new GooglePage(Driver);
        private DriverHandler B => new DriverHandler(Driver);
        //private SeleniumHandler Selenium => new SeleniumHandler(Driver);

        [TestInitialize]
        public void Setup()
        {
            Driver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
            //Selenium = new SeleniumHandler(Driver);
            //Browser = new DriverHandler(Driver);
            //Browser = new GoogleBrowser(BrowserType.Chrome);
            Test = new GoogleOperations(Driver);
            //Element = new ElementHandler(Browser.Driver);
            //Harness = new GoogleHarness();
        }

        [TestMethod]
        public void ExampleTest()
        {
            Driver.Navigate().GoToUrl(GooglePage.URL);
            Google.FrontPage.SearchInput.SendKeysWhenReady("selenium browser automation");
            Google.FrontPage.SeachButton.ClickWhenReady();
            Assert.IsTrue(Google.ResultsPage.SearchResultsDiv.WaitForVisibleState());
        }

        

        [TestMethod]
        public void SearchForSeleniumOnGoogle()
        {
            Test.NavigateToGoogle();
            Assert.IsTrue(Test.SearchForItem("selenium browser automation"), 
                "No search results displayed when they were expected");
            Assert.IsTrue(Test.FindSearchResultItem("SeleniumHQ Browser Automation"),
                "Did not find a specific search result for Selenium - Web Browser Automation");
        }

        [TestMethod]
        public void VerifySeleniumDoesNotExistForElement()
        {
            Test.NavigateToGoogle();
            Test.SearchForItem("selenium element");
            Assert.IsFalse(Test.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Found a specific search result for Selenium - Web Browser Automation when none was expected");
        }

        [TestMethod]
        public void ReturnToHomepage()
        {
            Test.NavigateToGoogle();
            Test.SearchForItem("selenium automation");
            Test.GoToHomePage();
            //Assert.IsTrue(Element.WaitForInvisibleState(ResultsPage.SearchResultsDiv),
            //    "Search results displayed when they were not expected");
        }

        [TestMethod]
        public void TakeScreenshotSavesFile()
        {
            string path = Path.Combine(Path.GetTempPath(), "screenshots");
            if(Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Test.NavigateToGoogle();
            Test.SearchForItem("selenium automation");
            B.TakeScreenshot();
            var files = Directory.GetFiles(path);
            Assert.AreEqual(1, files.Length);

            Directory.Delete(path, true);
        }

        [TestMethod]
        public void TakeScreenshotWithPathSavesFile()
        {
            FileInfo file = new FileInfo(
                Path.Combine(Directory.GetCurrentDirectory(),
                "screenshot",
                $"{((RemoteWebDriver)Driver).Capabilities.GetCapability("browserName")}_override_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png"));
            if (Directory.Exists(file.Directory.FullName))
            {
                Directory.Delete(file.Directory.FullName, true);
            }

            Test.NavigateToGoogle();
            Test.SearchForItem("selenium automation");

            Directory.CreateDirectory(file.DirectoryName);
            B.SetScreenshotLocation(file).TakeScreenshot();
            var files = Directory.GetFiles(file.Directory.FullName);
            Assert.AreEqual(1, files.Length);

            Directory.Delete(file.Directory.FullName, true);
        }

        [TestCleanup]
        public void Teardown()
        {
            Driver.Dispose();
        }
    }
}
