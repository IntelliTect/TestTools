using IntelliTect.TestTools.TestFramework;
using OpenQA.Selenium;
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
                .AddTestCaseService<Harness.IntelliTectWebpage>()
                .AddTestBlock<TestBlocks.NavigateToWebsite>()
                .AddTestBlock<TestBlocks.VerifyWebsiteBodyIsDisplayed>(expectedResult)
                .ExecuteTestCase();
        }

        [Fact]
        public void RegisterMembership()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddTestCaseService<IWebDriver>(new WebDriverFactory("Chrome").Driver)
                .AddTestCaseService<Harness.IntelliTectWebpage>()
                .AddTestBlock<TestBlocks.RegisterAndReturnNewAccount>("Tester", "McTesterson")
                .AddTestBlock<TestBlocks.VerifyAccountRegisteredCorrectly>()
                .ExecuteTestCase();
        }

        [Fact]
        public void LogIn()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddTestCaseService<IWebDriver>(new WebDriverFactory("Chrome").Driver)
                .AddTestCaseService<Account>(new AccountFactory().Account)
                .AddTestCaseService<Harness.IntelliTectWebpage>()
                .AddTestBlock<TestBlocks.LogIn>()
                .AddTestBlock<TestBlocks.VerifyAccountSummary>()
                .ExecuteTestCase();
        }
    }
}