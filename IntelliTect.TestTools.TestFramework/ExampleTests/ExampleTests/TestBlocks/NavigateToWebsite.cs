using ExampleTests.Data;
using OpenQA.Selenium;

namespace ExampleTests.TestBlocks
{
    public class NavigateToWebsite : TestBlockBase
    {
        public void Execute()
        {
            IntelliTect.HomePage.NavigateToPage();
        }
    }
}
