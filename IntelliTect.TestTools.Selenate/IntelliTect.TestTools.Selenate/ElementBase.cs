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

        /// <summary>
        /// The locator used to find IWebElements in this handler.
        /// </summary>
        public By Locator { get; protected set; }
        protected ISearchContext SearchContext { get; set; }
    }
}
