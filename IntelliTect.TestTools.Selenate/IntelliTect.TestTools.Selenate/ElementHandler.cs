using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Reflection;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Main class for handling interactions with a specific IWebElement.
    /// </summary>
    public class ElementHandler : HandlerBase
    {
        /// <summary>
        /// Takes an IWebDriver and a Selenium By locator used for operations with this element.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public ElementHandler(IWebDriver driver, By locator) : base(driver)
        {
            Locator = locator;
        }

        /// <summary>
        /// Takes an IWebDriver used for operations with this element. Must call SetLocator on this before operations will function.
        /// </summary>
        /// <param name="driver"></param>
        public ElementHandler(IWebDriver driver) : base(driver) { }

        public  By Locator { get; private set; }

        private bool _IgnoreExceptions;

        /// <summary>
        /// Sets the locator to use for operations within this instance.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public ElementHandler SetLocator(By by)
        {
            Locator = by;
            return this;
        }

        /// <summary>
        /// Sets the timeout to use when retrying operations within this instance.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ElementHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<ElementHandler>(timeout);
        }

        /// <summary>
        /// Sets the timeout in seconds to use when retrying operations within this instance.
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public ElementHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<ElementHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// Sets the polling interval to use when retrying operations within this instance.
        /// </summary>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
        public ElementHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<ElementHandler>(pollingInterval);
        }

        /// <summary>
        /// Sets the polling interval in seconds to use when retrying operations within this instance.
        /// </summary>
        /// <param name="pollIntervalInMilliseconds"></param>
        /// <returns></returns>
        public ElementHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<ElementHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        /// <summary>
        /// Ignores all exceptions of type WebDriverException when trying operations within this instance. This should be used as sparingly as possible.
        /// </summary>
        public ElementHandler IgnoreAllWebdriverExceptions(bool shouldIgnoreExceptions = true)
        {
            _IgnoreExceptions = shouldIgnoreExceptions;
            return this;
        }

        /// <summary>
        /// Clicks on the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <returns></returns>
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

            wait.Until(d =>
            {
                d.FindElement(Locator).Click();
                return true;
            });
        }

        /// <summary>
        /// Sends keys to the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <param name="textToSend"></param>
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

            wait.Until(d =>
            {
                IWebElement elem = d.FindElement(Locator);
                elem.SendKeys(textToSend);
                return true;
            });
        }

        /// <summary>
        /// Clears text in the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        public void Clear()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(StaleElementReferenceException)
                );

            wait.Until(d =>
            {
                IWebElement elem = d.FindElement(Locator);
                elem.Clear();
                return true;
            });
        }

        // Need to find what to actually do here.
        // This should either be:
        // 1. Deleted because the current element is found by all of the other methods that require it
        // 2. Reworked to match IWebElement.FindElement() to allow element chaining (but that has its own issues and we'd have to keep track of every parent By.)
        // 3. Just return "this" which seems unnecessary and superfluous
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IWebElement FindElement()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException)
                );

            return wait.Until(d =>
            {
                IWebElement elem = d.FindElement(Locator);
                if (elem.Displayed) return elem;
                return null;
            });
        }

        /// <summary>
        /// Gets the existing text on the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException)
                );

            return wait.Until(d =>
            {
                IWebElement elem = d.FindElement(Locator);
                return elem.Text;
            });
        }

        /// <summary>
        /// Gets a specific attribute of the element found by locator <see cref="SetLocator(By)"/> or <seealso cref="ElementHandler(IWebDriver, By)"/>. Will automatically retry if a known failure occurs.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public string GetAttribute(string attributeName)
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(StaleElementReferenceException)
                );

            return wait.Until(d =>
            {
                IWebElement elem = d.FindElement(Locator);
                return elem.GetAttribute(attributeName);
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
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(Locator);
                    return elem.Displayed;
                });
            }
            catch (WebDriverTimeoutException ex)
                when (ex.InnerException is not StaleElementReferenceException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForNotDisplayed()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(Locator);
                    return !elem.Displayed;
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
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForEnabled()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(Locator);
                    return elem.Enabled;
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
        /// Waits for the element to be enabled.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForDisabled()
        {
            IWait<IWebDriver> wait = ElementWait();
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(Locator);
                    return !elem.Enabled;
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
