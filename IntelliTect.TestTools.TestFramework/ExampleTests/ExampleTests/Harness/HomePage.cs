using OpenQA.Selenium;

namespace ExampleTests.Harness
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver) { }

        public void NavigateToPage()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
        }

        public IWebElement Body => Driver.FindElement(By.TagName("body"));
    }
}
