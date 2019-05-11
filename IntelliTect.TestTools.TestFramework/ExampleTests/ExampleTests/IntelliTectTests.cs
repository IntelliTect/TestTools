using ExampleTests.TestBlocks;
using IntelliTect.TestTools.TestFramework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using Xunit;

namespace ExampleTests
{
    public class IntelliTectTests : IDisposable
    {
        public IntelliTectTests()
        {
            //ChromeOptions chromeOptions = new ChromeOptions();
            //chromeOptions.AddArgument("--disable-extension");
            //chromeOptions.AddArgument("--no-sandbox");
            //chromeOptions.AddArgument("--disable-infobars");
            //chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            //chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            // Change to factory
            Driver = new ChromeDriver(Directory.GetCurrentDirectory()/*, chromeOptions*/);
        }

        [Fact]
        public void Test1()
        {
            var expectedResult = new Data.Expected.SiteStatus { IsAvailable = true };

            // Is there a better way to do this to show the inputs and outputs of test blocks?

            TestBuilder builder = new TestBuilder();
            builder
                .AddData(Driver)
                .AddTestBlock<TestBlocks.NavigateToWebsite>()
                .AddData(expectedResult)
                .AddTestBlock<TestBlocks.VerifyWebsiteAvailability>()
                .ExecuteTestCase();
        }

        public void Dispose()
        {
            Driver.Dispose();
        }

        private IWebDriver Driver { get; set; }
    }
}
