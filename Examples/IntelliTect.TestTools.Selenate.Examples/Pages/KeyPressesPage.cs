using OpenQA.Selenium;

namespace IntelliTect.TestTools.Selenate.Examples.Pages
{
    public class KeyPressesPage
    {
        public KeyPressesPage(IWebDriver driver)
        {
            _Driver = driver;
        }

        public ElementHandler InputBox => new ElementHandler(_Driver, By.Id("target"));
        public ElementHandler ResultText => new ElementHandler(_Driver, By.Id("result"));

        private IWebDriver _Driver;
    }
}
