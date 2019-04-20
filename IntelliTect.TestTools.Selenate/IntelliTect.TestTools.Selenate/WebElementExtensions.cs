using System;
using IntelliTect.TestTools.Selenate;
using OpenQA.Selenium;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;

namespace IntelliTect.TestTools.SelenateExtensions
{
    public static class WebElementExtensions
    {
        /// <summary>
        /// Attempts to find a child element of this element, only returning when the child element is found OR throws when a timeout is reached
        /// </summary>
        /// <param name="by">The selenium By statement for the child element</param>
        public static IWebElement FindElementWhenReady(this IWebElement element, IWebDriver driver, By by, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));

            return wait.Until( f => element.FindElement( by ) );
        }

        /// <summary>
        /// Attempts to find all child elements matching a certian criteria of this element, only returning when at least one child element is found OR throws when a timeout is reached
        /// </summary>
        /// <param name="by">The selenium By statement for the child element</param>
        public static IReadOnlyCollection<IWebElement> FindElementsWhenReady(this IWebElement element, IWebDriver driver, By by, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));

            return wait.Until(f => element.FindElements(by));
        }

        /// <summary>
        /// Scrolls the current element a certain number of pixels down from the top of the screen. Primarily used to get around headers that cover up elements
        /// </summary>
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
        /// <param name="value">Value to send to the element</param>
        public static void SendKeysAndTabWhenReady(this IWebElement element, IWebDriver driver, string value, int secondsToTry = 5)
        {
            SendKeysWhenReady(element, driver, value, secondsToTry );
            element.SendKeys(Keys.Tab);
        }

        /// <summary>
        /// Waits for the element to be in a valid state, 
        /// then clears the current text and uses SendKeys to send the specified value to the element 
        /// or throws after a certain amount of time
        /// </summary>
        /// <param name="value">Value to send to the element</param>
        public static void SendKeysWhenReady(this IWebElement element, IWebDriver driver, string value, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(InvalidElementStateException));

            for ( int i = 0; i < 5; i++ )
            {
                wait.Until( c =>
                {
                    element.Clear();
                    return true;
                } );

                wait.Until( s =>
                {
                    element.SendKeys( value );
                    return true;
                } );

                if ( element.GetAttribute( "value" ) == value )
                    return;
            }
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then clicks on it or throws after a certain amount of time
        /// </summary>
        //public static void ClickWhenReady(this IWebElement element, IWebDriver driver, int secondsToTry = 5)
        //{
        //    WebDriverWait wait = new WebDriverWait( driver, TimeSpan.FromSeconds( secondsToTry ) );
        //    wait.IgnoreExceptionTypes(
        //        typeof(ElementNotVisibleException),
        //        typeof(ElementNotInteractableException),
        //        typeof(StaleElementReferenceException),
        //        typeof(InvalidElementStateException),
        //        typeof(ElementClickInterceptedException));

        //    wait.Until( c =>
        //    {
        //        element.Click();
        //        return true;
        //    } );
        //}
    }
}
