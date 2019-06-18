using ExampleTests.Data;
using OpenQA.Selenium;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class VerifyWebsiteBodyIsDisplayed : TestBlockBase
    {
        //public Account Account { get; set; }
        //public Car Car { get; set; }
        //protected Data.SiteStatus ExpectedResult { get; set; }

        public void Execute(Data.SiteStatus expected)
        {
            Assert.Equal(expected.IsBodyAvailable, IntelliTect.HomePage.Body.Displayed);
        }
    }
}
