using OpenQA.Selenium;

namespace IntelliTect.TestTools.Selenate
{
    public class ElementBase : HandlerBase
    {
        public ElementBase(IWebDriver driver, By locator) : base(driver)
        {
            Locator = locator;
        }

        public IWebElement? ParentElement { get; internal set; }
        public By Locator { get; protected set; }
    }
}
