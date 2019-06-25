using System;
using OpenQA.Selenium;
using System.Threading.Tasks;
using System.Collections.Generic;
using IntelliTect.IntelliWait;

namespace IntelliTect.TestTools.SelenateExtensions
{
    // Warnings disabled as this class will be getting deprecated soon
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public static class WebElementExtensions
    {
        /// <summary>
        /// Attempts to find a child element of this element, only returning when the child element is found OR throws when a timeout is reached
        /// </summary>
        /// <param name="element"></param>
        /// <param name="by">The selenium By statement for the child element</param>
        /// <param name="secondsToTry"></param>
        public static Task<IWebElement> FindElementWhenReady(this IWebElement element, By by, int secondsToTry = 5)
        {
#pragma warning disable 618
            return Wait.Until<NoSuchElementException, StaleElementReferenceException, IWebElement>(
#pragma warning restore 618
                () => element.FindElement(by), TimeSpan.FromSeconds(secondsToTry));
        }

        /// <summary>
        /// Attempts to find all child elements matching a certian criteria of this element, only returning when at least one child element is found OR throws when a timeout is reached
        /// </summary>
        /// <param name="element"></param>
        /// <param name="by">The selenium By statement for the child element</param>
        /// <param name="secondsToTry"></param>
        public static Task<IReadOnlyCollection<IWebElement>> FindElementsWhenReady(this IWebElement element, By by, int secondsToTry = 5)
        {
#pragma warning disable 618
            return Wait.Until<NoSuchElementException, StaleElementReferenceException, IReadOnlyCollection<IWebElement>>(
#pragma warning restore 618
                () => element.FindElements(by), TimeSpan.FromSeconds(secondsToTry));
        }

        /// <summary>
        /// Scrolls the current element a certain number of pixels down from the top of the screen. Primarily used to get around headers that cover up elements
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <param name="pixelsFromTopOfScreen">The number of pixels to scroll from the top of the screen. More will put the element farther down on the screen</param>
        public static void ScrollIntoView(this IWebElement element, IWebDriver driver, int pixelsFromTopOfScreen = 200)
        {
            int position = element.Location.Y - pixelsFromTopOfScreen;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript($"window.scrollTo(0,{position})");
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then clears the current text, 
        /// uses SendKeys to send the specified value to the element, 
        /// then tabs out of the field or throws after a certain amount of time
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value">Value to send to the element</param>
        public static async Task FillInWithAndTabWhenReady(this IWebElement element, string value)
        {
            await element.FillInWithWhenReady(value);
            // Worth wrapping the below in a wait?
            element.SendKeys(Keys.Tab);
        }

        /// <summary>
        /// Waits for the element to be in a valid state, 
        /// then clears the current text and uses SendKeys to send the specified value to the element 
        /// or throws after a certain amount of time
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value">Value to send to the element</param>
        /// <param name="secondstoTry"></param>
        public static async Task FillInWithWhenReady(this IWebElement element, string value, int secondstoTry = 5)
        {
            var count = 0;
            while (element.GetAttribute("value") != value && count < 5)
            {
#pragma warning disable 618
                await Wait.Until<NoSuchElementException,
                    ElementNotVisibleException,
                    StaleElementReferenceException>(
                    () => element.Clear(), TimeSpan.FromSeconds(secondstoTry));
                await Wait.Until<NoSuchElementException,
                    ElementNotVisibleException,
                    ElementNotInteractableException,
                    StaleElementReferenceException>(
                    () => element.SendKeys(value), TimeSpan.FromSeconds(secondstoTry));
#pragma warning restore 618
                count++;
            }
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then clicks on it or throws after a certain amount of time
        /// </summary>
        public static Task ClickWhenReady(this IWebElement element, int secondsToTry = 5)
        {
#pragma warning disable 618
            return Wait.Until<
#pragma warning restore 618
                NoSuchElementException,
                ElementNotVisibleException,
                ElementClickInterceptedException,
                StaleElementReferenceException>(() => element.Click(), TimeSpan.FromSeconds(secondsToTry));
        }
    }
}
