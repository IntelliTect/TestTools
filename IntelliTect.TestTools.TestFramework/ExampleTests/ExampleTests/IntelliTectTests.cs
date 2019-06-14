using IntelliTect.TestTools.TestFramework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using Xunit;
using ExampleTests.Data;

namespace ExampleTests
{
    public class IntelliTectTests
    {
        [Fact]
        public void Test1()
        {
            var expectedResult = new SiteStatus
                {
                    IsHeaderAvailable = true,
                    IsBodyAvailable = true
                };

            TestBuilder builder = new TestBuilder();
            builder
                .AddTestCaseService<IWebDriver>(new WebDriverFactory("Chrome").Driver)
                .AddTestCaseService<Account>(new AccountFactory().AccountId)
                .AddTestCaseService<Pages.HomePage>()
                .AddTestCaseService<Pages.Blogs>()
                .AddTestBlock<TestBlocks.NavigateToWebsite>()
                //.AddTestBlock<TestBlocks.VerifyWebsiteBodyIsDisplayed>(expectedResult)
                .AddTestBlock(new TestBlocks.VerifyWebsiteBodyIsDisplayed(expectedResult))
                //.AddDependencyInstance(expectedResult)
                //.AddTestBlock<TestBlocks.VerifyWebsiteBodyIsDisplayed>()
                //.AddTestBlock<TestBlocks.VerifyWebsiteBodyIsDisplayed>("1", "2", "3")
                .ExecuteTestCase();
        }
    }
}