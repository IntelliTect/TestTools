using OpenQA.Selenium;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.SelenateExtensions
{
    public static class WebElementExtensions
    {
        public static void ScrollIntoView(this IWebElement element, IWebDriver driver)
        {
            int position = element.Location.Y - 200;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string title = (string)js.ExecuteScript($"window.scrollTo(0,{position})");
            // Give the browser and javascript a chance to execute.
            Task.Delay(50).Wait();
        }

        public static void FillInWithAndTab(this IWebElement element, string value)
        {
            element.FillInWith(value);
            element.SendKeys(Keys.Tab);
            // Wait afterward to let websites register the tab
            Task.Delay(500).Wait();
        }

        public static void FillInWith(this IWebElement element, string value)
        {
            var count = 0;
            while (element.GetAttribute("value") != value && count < 5)
            {
                element.Clear();
                element.SendKeys(value);
            }
        }
    }
}
