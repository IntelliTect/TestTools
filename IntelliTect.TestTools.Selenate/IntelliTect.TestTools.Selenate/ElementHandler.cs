using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Class for handling checking for state or polling specific elements utilizing common WebDriverWait implementations
    /// </summary>
    public class ElementHandler
    {
        /// <summary>
        /// Constructor for handling the driver used to create WebDriverWaits
        /// </summary>
        /// <param name="driver">The driver to use for polling</param>
        public ElementHandler(IWebDriver driver)
        {
            this.Driver = driver;
        }

        /// <summary>
        /// 
        /// </summary>
        public ElementHandler(Browser browser)
        {
            Browser = browser;
        }

        private IWebDriver Driver { get; }
        private Browser Browser { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="secondsToTry"></param>
        public void ClickElementWhenReady(By locator, int secondsToTry = 5)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(ElementClickInterceptedException)
                );
            // Worth wrapping in a try/catch and returning false if not successful?
            wait.Until(d =>
            {
                if(Browser.TryFindElement(locator, out IWebElement elemt))
                {
                    elemt.Click();
                    return true;
                }
                d.FindElement(locator).Click();
                return true;
            });
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then clicks on it.
        /// </summary>
        /// <param name="element">The IWebElement implementation to perform a Click on</param>
        /// <param name="secondsToTry">The seconds to wait for the element to be in a valid state before failing</param>
        [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
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
            wait.Until(_ =>
            {
                element.Click();
                return true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="textToSend"></param>
        /// <param name="secondsToTry"></param>
        public void SendKeysWhenReady(By locator, string textToSend, int secondsToTry = 5)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(InvalidElementStateException),
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(ElementNotInteractableException)
                );
            // Worth wrapping in a try/catch and returning false if not successful?
            wait.Until(d =>
            {
                IWebElement elem = d.FindElement(locator);
                elem.Clear();
                elem.SendKeys(textToSend);
                System.Threading.Tasks.Task.Delay(100).Wait();
                return elem.GetAttribute("value") == textToSend;
            });
        }

        /// <summary>
        /// Waits for the element to be in a valid state, then performs a SendKeys to it.
        /// </summary>
        /// <param name="element">The IWebElement implementation to perform a SendKeys on</param>
        /// <param name="textToSend">The text to send to the element</param>
        /// <param name="secondsToTry">The seconds to wait for the element to be in a valid state before failing</param>
        [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
        public void SendKeysWhenReady(IWebElement element, string textToSend, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(InvalidElementStateException),
                typeof(NoSuchElementException));

            wait.Until(_ => {
                element.Clear();
                element.SendKeys(textToSend);
                System.Threading.Tasks.Task.Delay(250).Wait();
                return element.GetAttribute("value") == textToSend;
            });
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <param name="locator">The IWebElement implementation to check for visibility state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForVisibleState(By locator, int secondsToTry = 5)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d => 
                {
                    IWebElement elem = d.FindElement(locator);
                    return elem.Displayed;
                });
            }
            catch (WebDriverException ex)
                when (ex.InnerException is NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <param name="locator">The IWebElement implementation to check for visibility state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForInvisibleState(By locator, int secondsToTry = 5)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(locator);
                    return !elem.Displayed;
                });
            }
            catch (WebDriverException ex)
                when (ex.InnerException is NoSuchElementException)
            {
                return true;
            }
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <param name="element">The IWebElement implementation to check for visibility state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
        public bool WaitForVisibleState(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotVisibleException),
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(_ => element.Displayed);
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
        /// <param name="locator">The IWebElement implementation to check for enabled state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForEnabledState(By locator, int secondsToTry = 5)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(locator);
                    return elem.Enabled;
                });
            }
            catch (WebDriverException ex)
                when (ex.InnerException is NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be enabled.
        /// </summary>
        /// <param name="locator">The IWebElement implementation to check for enabled state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForDisabledState(By locator, int secondsToTry = 5)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
            wait.PollingInterval = TimeSpan.FromMilliseconds(100);
            wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException)
                );

            try
            {
                return wait.Until(d =>
                {
                    IWebElement elem = d.FindElement(locator);
                    return !elem.Enabled;
                });
            }
            catch (WebDriverException ex)
                when (ex.InnerException is NoSuchElementException)
            {
                return true;
            }
        }

        /// <summary>
        /// Waits for the element to be enabled.
        /// </summary>
        /// <param name="element">The IWebElement implementation to check for enabled state</param>
        /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
        public bool WaitForEnabledState(IWebElement element, int secondsToTry = 5)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
            wait.IgnoreExceptionTypes(
                typeof(ElementNotInteractableException),
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(_ => element.Enabled);
            }
            catch(WebDriverException ex)
                when (ex.InnerException is ElementNotInteractableException)
            {
                return false;
            }
        }
    }
}
