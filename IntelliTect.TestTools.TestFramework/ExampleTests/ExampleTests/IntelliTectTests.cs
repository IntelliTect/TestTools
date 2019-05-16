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
            // Change to factory
            Driver = new ChromeDriver(Directory.GetCurrentDirectory());
        }

        [Fact]
        public void Test1()
        {
            var expectedResult = new Data.Expected.SiteStatus { IsAvailable = true };

            // Is there a better way to do this to show the inputs and outputs of test blocks?

            TestBuilder builder = new TestBuilder();
            builder
                .AddItemToBag(Driver)
                .AddTestBlock<TestBlocks.NavigateToWebsite>()
                .AddItemToBag(expectedResult)
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
