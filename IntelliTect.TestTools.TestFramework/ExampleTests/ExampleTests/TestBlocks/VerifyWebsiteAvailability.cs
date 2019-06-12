using ExampleTests.Data;
using OpenQA.Selenium;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class VerifyWebsiteBodyIsDisplayed : TestBlockBase
    {
        public VerifyWebsiteBodyIsDisplayed(IWebDriver driver) : base(driver){ }

        public Account Account { get; set; }
        public Car Car { get; set; }

        public void Execute(Data.SiteStatus expected)
        {
            var body = Driver.FindElement(By.TagName("body"));
            Assert.Equal(expected.IsBodyAvailable, body.Displayed);
        }
    }
}
