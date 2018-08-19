using OpenQA.Selenium;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.SelenateExtensions
{
    public static class WebElementExtensions
    {
        public static async void ScrollIntoView(this IWebElement element, IWebDriver driver)
        {
            int position = element.Location.Y - 200;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string title = (string)js.ExecuteScript($"window.scrollTo(0,{position})");
            // Give the browser and javascript a chance to execute.
            // In the middle of a refactor. Eventually change this out for a centralized wait
            await Task.Delay(50);
        }

        public static async void FillInWithAndTab(this IWebElement element, string value)
        {
            element.FillInWith(value);
            element.SendKeys(Keys.Tab);
            // Wait afterward to let websites register the tab
            // In the middle of a refactor. Eventually change this out for a centralized wait
            await Task.Delay(250);
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
    }
}
