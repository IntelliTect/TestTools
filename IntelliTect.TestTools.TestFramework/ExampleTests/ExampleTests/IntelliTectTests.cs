using IntelliTect.TestTools.TestFramework;
using OpenQA.Selenium;
using Xunit;
using ExampleTests.Data;

namespace ExampleTests
{
    public class IntelliTectTests
    {
        // Convert to tests for real websites.
        // For scenarios I can't do on IntelliTect's site, try:
        // http://the-internet.herokuapp.com/
        // https://www.ultimateqa.com/automation/
        // https://demoqa.com/
        // http://automationpractice.com/index.php
        // https://www.telerik.com/support/demos <-- Might be good for multi-language testing? Check out ConversationUI
        // https://restful-booker.herokuapp.com/ <-- for API tests

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
                .AddDependencyService<IWebDriver>(new WebDriverFactory("Chrome").Driver)
                .AddDependencyService<Harness.IntelliTectWebpage>()
                .AddTestBlock<TestBlocks.NavigateToWebsite>()
                .AddTestBlock<TestBlocks.VerifyWebsiteBodyIsDisplayed>(expectedResult)
                .ExecuteTestCase();
        }

        [Fact]
        public void RegisterMembership()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<IWebDriver>(new WebDriverFactory("Chrome").Driver)
                .AddDependencyService<Harness.IntelliTectWebpage>()
                .AddTestBlock<TestBlocks.RegisterAndReturnNewAccount>("NewTester", "McTest")
                .AddTestBlock<TestBlocks.VerifyAccountRegisteredCorrectly>()
                .ExecuteTestCase();
        }

        [Fact]
        public void LogIn()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyService<IWebDriver>(new WebDriverFactory("Chrome").Driver)
                .AddDependencyService<Account>(new AccountFactory().Account)
                .AddDependencyService<Harness.IntelliTectWebpage>()
                .AddTestBlock<TestBlocks.LogIn>()
                .AddTestBlock<TestBlocks.VerifyAccountSummary>()
                .ExecuteTestCase();
        }
    }
}