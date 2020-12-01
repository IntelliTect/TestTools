using System;
using IntelliTect.TestTools.Selenate;
using System.IO;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using Xunit;

namespace GoogleSearch
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class ExampleTests : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
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

        public ExampleTests()
        {
            Driver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
            //Selenium = new SeleniumHandler(Driver);
            //Browser = new DriverHandler(Driver);
            //Browser = new GoogleBrowser(BrowserType.Chrome);
            Test = new GoogleOperations(Driver);
            //Element = new ElementHandler(Browser.Driver);
            //Harness = new GoogleHarness();
        }

        [Fact]
        public void ExampleTest()
        {
            Driver.Navigate().GoToUrl(GooglePage.URL);
            Google.FrontPage.SearchInput.SendKeysWhenReady("selenium browser automation");
            Google.FrontPage.SeachButton.ClickWhenReady();
            Assert.True(Google.ResultsPage.SearchResultsDiv.WaitForVisibleState());
        }

        

        [Fact]
        public void SearchForSeleniumOnGoogle()
        {
            Test.NavigateToGoogle();
            Assert.True(Test.SearchForItem("selenium browser automation"), 
                "No search results displayed when they were expected");
            Assert.True(Test.FindSearchResultItem("SeleniumHQ Browser Automation"),
                "Did not find a specific search result for Selenium - Web Browser Automation");
        }

        [Fact]
        public void VerifySeleniumDoesNotExistForElement()
        {
            Test.NavigateToGoogle();
            Test.SearchForItem("selenium element");
            Assert.False(Test.FindSearchResultItem("Selenium - Web Browser Automation"),
                "Found a specific search result for Selenium - Web Browser Automation when none was expected");
        }

        [Fact]
        public void ReturnToHomepage()
        {
            Test.NavigateToGoogle();
            Test.SearchForItem("selenium automation");
            Test.GoToHomePage();
            //Assert.True(Element.WaitForInvisibleState(ResultsPage.SearchResultsDiv),
            //    "Search results displayed when they were not expected");
        }

        [Fact]
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
            Assert.Single(files);

            Directory.Delete(path, true);
        }

        [Fact]
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
            Assert.Single(files);

            Directory.Delete(file.Directory.FullName, true);
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            Driver.Dispose();
            
        }
    }
}
