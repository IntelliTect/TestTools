using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Main class for handling interactions with a specific IWebElement.
    /// </summary>
    public class ElementHandler : ElementBase
    {
        /// <summary>
        /// Takes an IWebDriver and a Selenium By locator used for operations with this element.
        /// </summary>
        /// <param name="driver">The WebDriver to wrap.</param>
        /// <param name="locator">Method for locating an element.</param>
        public ElementHandler(IWebDriver driver, By locator) : base(driver, locator)
        {
        }

        private bool _IgnoreExceptions;

        /// <summary>
        /// Sets the locator to use for operations within this instance.
        /// </summary>
        /// <param name="by">Method to find an element.</param>
        /// <returns>this</returns>
        public ElementHandler SetLocator(By by)
        {
            Locator = by;
            return this;
        }

        /// <summary>
        /// Sets the timeout to use when retrying operations within this instance.
        /// </summary>
        /// <param name="timeout">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        public ElementHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<ElementHandler>(timeout);
        }

        /// <summary>
        /// Sets the timeout in seconds to use when retrying operations within this instance.
        /// </summary>
        /// <param name="timeoutInSeconds">Duration to retry an action before throwing.</param>
        /// <returns>this</returns>
        public ElementHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<ElementHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// Sets the polling interval to use when retrying operations within this instance.
        /// </summary>
        /// <param name="pollingInterval">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        public ElementHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<ElementHandler>(pollingInterval);
        }

        /// <summary>
        /// Sets the polling interval in seconds to use when retrying operations within this instance.
        /// </summary>
        /// <param name="pollIntervalInMilliseconds">Time to wait in between retrying an action.</param>
        /// <returns>this</returns>
        public ElementHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<ElementHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        public ElementHandler SetSearchContext(ISearchContext searchContext)
        {
            SearchContext = searchContext;
            return this;
        }

        /// <summary>
        /// Ignores all exceptions of type WebDriverException when trying operations within this instance. This should be used as sparingly as possible.
        /// </summary>
        /// <returns>this</returns>
        public ElementHandler IgnoreAllWebdriverExceptions(bool shouldIgnoreExceptions = true)
        {
            _IgnoreExceptions = shouldIgnoreExceptions;
            return this;
        }

        public ElementHandler FindElement(By by)
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            IWebElement foundElem = wait.Until(_ => {
                return SearchContext.FindElement(Locator);
            });

            ElementHandler newHandler = new(WrappedDriver, by);
            newHandler.SetSearchContext(foundElem);

            return newHandler;
        }

        public ElementsHandler FindElements(By by)
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            IWebElement foundElem = wait.Until(_ => {
                return SearchContext.FindElement(Locator);
            });

            ElementsHandler newHandler = new(WrappedDriver, by);
            newHandler.SetSearchContext(foundElem);

            return newHandler;
        }

        /// <summary>
        /// Clicks on the element found by <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        public void Click()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(ElementClickInterceptedException)
                );

            wait.Until(_ =>
            {
                SearchContext.FindElement(Locator).Click();
                return true;
            });
        }

        /// <summary>
        /// Sends keys to the element found by <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <param name="textToSend">Text to send to the element.</param>
        public void SendKeys(string textToSend)
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(ElementNotInteractableException)
                );

            wait.Until(_ =>
            {
                SearchContext.FindElement(Locator).SendKeys(textToSend);
                return true;
            });
        }

        /// <summary>
        /// Clears text in the element found by <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        public void Clear()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(StaleElementReferenceException)
                );

            wait.Until(_ =>
            {
                SearchContext.FindElement(Locator).Clear();
                return true;
            });
        }

        /// <summary>
        /// Finds and returns the element found by <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>.
        /// Subsequent actions will not automatically retry on the returned IWebElement.
        /// </summary>
        /// <returns>The IWebElement found.</returns>
        public IWebElement GetWebElement()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

#pragma warning disable CS8603 // Possible null reference return. Needed for proper WebDriverWait behavior
            return wait.Until(_ =>
            {
                IWebElement elem = SearchContext.FindElement(Locator);
                if (elem.Displayed) return elem;
                return null;
            });
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Gets the existing text on the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <returns>The text associated to the found element.</returns>
        public string Text()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException)
                );

            return wait.Until(_ =>
            {
                return SearchContext.FindElement(Locator).Text;
            });
        }

        /// <summary>
        /// Gets a specific attribute of the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <param name="attributeName">The element attribute to search for.</param>
        /// <returns>The value of the found attribute.</returns>
        public string GetAttribute(string attributeName)
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(StaleElementReferenceException)
                );

            return wait.Until(_ =>
            {
                return SearchContext.FindElement(Locator).GetAttribute(attributeName);
            });
        }

        /// <summary>
        /// Waits for the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/> to be displayed. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <returns>True if the element is displayed, false if the the element is not displayed or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForDisplayed()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(_ =>
                {
                    return SearchContext.FindElement(Locator).Displayed;
                });
            }
            catch (WebDriverTimeoutException ex)
                when (ex.InnerException is not StaleElementReferenceException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element found by <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/> to be NOT displayed.
        /// </summary>
        /// <returns>True if the element is NOT displayed, false if the the element is displayed or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForNotDisplayed()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(_ =>
                {
                    return !SearchContext.FindElement(Locator).Displayed;
                });
            }
            catch (NoSuchElementException)
            {
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be enabled.
        /// </summary>
        /// <returns>True if the element is enabled, false if the the element is not disabled.</returns>
        public bool WaitForEnabled()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(_ =>
                {
                    return SearchContext.FindElement(Locator).Enabled;
                });
            }
            // A Null inner exception implies the element was found but was in a disabled state
            catch (WebDriverTimeoutException ex)
                when (ex.InnerException is null) 
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be disabled.
        /// </summary>
        /// <returns>True if the element is disabled, false if the the element is enabled.</returns>
        public bool WaitForDisabled()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(_ =>
                {
                    return !SearchContext.FindElement(Locator).Enabled;
                });
            }
            // A Null inner exception implies the element was found but was in an enabled state
            catch (WebDriverTimeoutException ex)
                when (ex.InnerException is null)
            {
                return false;
            }
            
        }

        private IWait<IWebDriver> ElementWait()
        {
            IWait<IWebDriver> wait = Wait;
            if (_IgnoreExceptions)
            {
                wait.IgnoreExceptionTypes(typeof(WebDriverException));
            }

            return wait;
        }
    }
}
