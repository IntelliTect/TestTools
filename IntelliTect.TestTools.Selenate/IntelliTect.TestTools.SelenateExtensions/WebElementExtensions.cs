using IntelliTect.TestTools.Selenate;
using OpenQA.Selenium;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.SelenateExtensions
{
    public static class WebElementExtensions
    {
        public static void ScrollIntoView(this IWebElement element, IWebDriver driver, int pixelsFromTopOfScreen = 200)
        {
            int position = element.Location.Y - pixelsFromTopOfScreen;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.scrollTo(0,{position})");
        }

        public static void FillInWithAndTab(this IWebElement element, string value)
        {
            element.FillInWith(value);
            element.SendKeys(Keys.Tab);
        }

        public static void FillInWith(this IWebElement element, string value)
        {
            var count = 0;
            while (element.GetAttribute("value") != value && count < 5)
            {
                element.Clear();
                element.SendKeys(value);
                count++;
            }
        }

        public static Task WaitAndClick( this IWebElement element )
        {
            ConditionalWait wait = new ConditionalWait();
            return wait.WaitFor<
                NoSuchElementException,
                ElementNotVisibleException,
                ElementClickInterceptedException>( () => element.Click() );
        }
    }
}
