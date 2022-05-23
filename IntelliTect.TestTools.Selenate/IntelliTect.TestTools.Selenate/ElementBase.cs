using OpenQA.Selenium;

namespace IntelliTect.TestTools.Selenate
{
    public class ElementBase : HandlerBase
    {
        public ElementBase(IWebDriver driver, By locator) : base(driver)
        {
            Locator = locator;
            SearchContext = driver;
        }

        // Do we need the ParentElement property if we keep track of SearchContext?
        //public IWebElement? ParentElement { get; internal set; }
        protected ISearchContext SearchContext { get; set; }
        public By Locator { get; protected set; }
    }
}
