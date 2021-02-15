using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Reflection;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementHandler : HandlerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public ElementHandler(IWebDriver driver, By locator) : base(driver)
        {
            //Driver = driver;
            Locator = locator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public ElementHandler(IWebDriver driver) : base(driver) { }

        public  By Locator { get; private set; }

        private bool _IgnoreExceptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public ElementHandler SetLocator(By by)
        {
            Locator = by;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ElementHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<ElementHandler>(timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public ElementHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<ElementHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
        public ElementHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<ElementHandler>(pollingInterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollIntervalInMilliseconds"></param>
        /// <returns></returns>
        public ElementHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<ElementHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        /// <summary>
        /// 
        /// </summary>
        public ElementHandler IgnoreAllWebdriverExceptions(bool shouldIgnoreExceptions = true)
        {
            _IgnoreExceptions = shouldIgnoreExceptions;
            return this;
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// Waits for the element to be visible.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
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
            catch (WebDriverTimeoutException)
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
