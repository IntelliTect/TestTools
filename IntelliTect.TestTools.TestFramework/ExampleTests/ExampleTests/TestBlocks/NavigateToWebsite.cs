using System.Diagnostics;
using OpenQA.Selenium;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class NavigateToWebsite : TestBlockBase
    {
        public NavigateToWebsite(IWebDriver driver) : base(driver)
        {
        }

        public Data.Actual.SiteStatus Execute()
        {
            HomePage.NavigateToPage();
            var body = Driver.FindElement(By.TagName("body"));
            var result = new Data.Actual.SiteStatus { IsAvailable = true };
            return result;
        }
    }
}
