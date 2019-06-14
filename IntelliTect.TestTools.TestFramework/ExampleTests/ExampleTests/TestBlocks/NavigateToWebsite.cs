using ExampleTests.Data;
using OpenQA.Selenium;

namespace ExampleTests.TestBlocks
{
    public class NavigateToWebsite : TestBlockBase
    {
        //public NavigateToWebsite(IWebDriver driver) : base(driver){ }

        public Account Account { get; set; }

        public Car Execute()
        {
            HomePage.NavigateToPage();
            return new Car { Make = "SomeCompany", Model = "Super fast model" };
        }
    }
}
