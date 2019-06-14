using OpenQA.Selenium;

namespace ExampleTests.Pages
{
    public class HomePage : BasePage
    {
        //public HomePage(IWebDriver driver) : base(driver)
        //{
        //}

        public void NavigateToPage()
        {
            Driver.Navigate().GoToUrl(BaseUrl);
        }
    }
}
