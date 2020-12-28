using OpenQA.Selenium;

namespace IntelliTect.TestTools.Selenate.Examples.Pages
{
    public class DynamicControlsPage
    {
        public DynamicControlsPage(IWebDriver driver)
        {
            _Driver = driver;
        }

        public ElementHandler RemoveAddButton => new ElementHandler(_Driver, By.CssSelector("button[onclick='swapCheckbox()']"));
        public ElementHandler Checkbox => new ElementHandler(_Driver, By.CssSelector("input[type='checkbox']"));
        public ElementHandler TextBox => new ElementHandler(_Driver, By.CssSelector("input[type='text']"));
        public ElementHandler EnableDisableButton => new ElementHandler(_Driver, By.CssSelector("button[onclick='swapInput()']"));

        private IWebDriver _Driver;
    }
}
