using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliTect.TestTools.Selenate
{
    public class Element
    {
        public Element(IWebDriver driver)
        {
            this.Driver = driver;
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then clicks on it.
        /// Exceptions:
        ///   T:OpenQA.Selenium.ElementNotVisibleException:
        ///     Thrown when the target element is not visible.
        ///
        ///   T:OpenQA.Selenium.StaleElementReferenceException:
        ///    Thrown when the target element is no longer valid in the document DOM.
        /// </summary>
        public void ClickElementWhenReady(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
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

        public void SendKeysWhenReady(IWebElement element, string textToSend, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
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

        public bool WaitForVisibleState(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(d => element.Displayed);
            }
            catch (WebDriverException ex)
                when (ex.InnerException.GetType() == typeof(ElementNotVisibleException)
                        || ex.InnerException.GetType() == typeof(NoSuchElementException))
            {
                return false;
            }
        }

        public bool WaitForEnabledState(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            return wait.Until(d => element.Enabled);
        }

        private IWebDriver Driver { get; set; }
    }
}
