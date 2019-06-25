using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliTect.TestTools.Selenate
{
    public class ElementHandler
    {
        public ElementHandler(IWebDriver driver)
        {
            this._Driver = driver;
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then clicks on it.
        /// </summary>
        /// <param name="element">The IWebElement implementation to perform a Click on</param>
        /// <param name="secondsToTry">The seconds to wait for the element to be in a valid state before failing</param>
        public void ClickElementWhenReady(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(InvalidElementStateException),
                typeof(ElementClickInterceptedException),
                typeof(NoSuchElementException));

            // Worth wrapping in a try/catch and throwing inner exception?
            wait.Until(c =>
            {
                element.Click();
                return true;
            });
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then performs a SendKeys to it.
        /// </summary>
        /// <param name="element">The IWebElement implementation to perform a SendKeys on</param>
        /// <param name="textToSend">The text to send to the element</param>
        /// <param name="secondsToTry">The seconds to wait for the element to be in a valid state before failing</param>
        public void SendKeysWhenReady(IWebElement element, string textToSend, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(InvalidElementStateException),
                typeof(NoSuchElementException));

            wait.Until(sk => {
                element.SendKeys(textToSend);
                return true;
            });
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <param name="element">The IWebElement implmentation to check for visibility state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForVisibleState(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(d => element.Displayed);
            }
            catch (WebDriverException ex)
                when (ex.InnerException is ElementNotVisibleException
                        || ex.InnerException is NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be enabled.
        /// </summary>
        /// <param name="element">The IWebElement implmentation to check for enabled state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForEnabledState(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(_Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(d => element.Enabled);
            }
            catch(WebDriverException ex)
                when (ex.InnerException.GetType() == typeof(ElementNotInteractableException))
            {
                return false;
            }            
        }

        private IWebDriver _Driver { get; set; }
    }
}
