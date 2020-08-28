using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

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

        private By Locator { get; set; }
        //private TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        //private TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        //private Type[] ExceptionsToIgnore { get; set; } = { typeof(NoSuchElementException), typeof(StaleElementReferenceException) };

        //private IWebDriver Driver { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ElementHandler SetTimeout(TimeSpan timeout)
        {
            base.Timeout = timeout;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
        public ElementHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            base.PollingInterval = pollingInterval;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ClickWhenReady()
        {
            IWait<IWebDriver> wait = Wait;
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
                d.FindElement(Locator).Click();
                return true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToSend"></param>
        public void SendKeysWhenReady(string textToSend)
        {
            IWait<IWebDriver> wait = Wait;
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
                IWebElement elem = d.FindElement(Locator);
                elem.SendKeys(textToSend);
                return true;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textToSend"></param>
        public void ReplaceTextWhenReady(string textToSend)
        {
            IWait<IWebDriver> wait = Wait;
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
                // NEED TO HANDLE KEYS.ENTER, KEYS.F1, ETC. HERE
                IWebElement elem = d.FindElement(Locator);
                elem.Clear();
                elem.SendKeys(textToSend);
                System.Threading.Tasks.Task.Delay(100).Wait();
                return elem.GetAttribute("value") == textToSend;
                //return true;
            });
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForVisibleState()
        {
            IWait<IWebDriver> wait = Wait;
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
                when (ex.InnerException is NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be visible.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForInvisibleState()
        {
            IWait<IWebDriver> wait = Wait;
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
        }

        /// <summary>
        /// Waits for the element to be enabled.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForEnabledState()
        {
            IWait<IWebDriver> wait = Wait;
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
            catch (WebDriverTimeoutException ex)
                when (ex.InnerException is NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the element to be enabled.
        /// </summary>
        /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
        public bool WaitForDisabledState()
        {
            IWait<IWebDriver> wait = Wait;
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
            catch (WebDriverTimeoutException ex)
                when (ex.InnerException is NoSuchElementException)
            {
                return true;
            }
        }
    }


    ///// <summary>
    ///// Class for handling checking for state or polling specific elements utilizing common WebDriverWait implementations
    ///// </summary>
    //public class ElementHandler1
    //{
    //    /// <summary>
    //    /// Constructor for handling the driver used to create WebDriverWaits
    //    /// </summary>
    //    /// <param name="driver">The driver to use for polling</param>
    //    public ElementHandler(IWebDriver driver)
    //    {
    //        this.Driver = driver;
    //    }

    //    private By Locator { get; set; }
    //    private TimeSpan Timeout { get; set; }
    //    private TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
    //    private IWebDriver Driver { get; }
    //    private List<Func<bool>> ActionsToPerform { get; }



    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="by"></param>
    //    /// <returns></returns>
    //    public ElementHandler Element(By by)
    //    {
    //        Locator = by;
    //        return this;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="cssSelector"></param>
    //    /// <returns></returns>
    //    public ElementHandler Element(string cssSelector)
    //    {
    //        Locator = By.CssSelector(cssSelector);
    //        return this;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="timeout"></param>
    //    /// <returns></returns>
    //    public ElementHandler SetTimeout(TimeSpan timeout)
    //    {
    //        Timeout = timeout;
    //        return this;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="pollingInterval"></param>
    //    /// <returns></returns>
    //    public ElementHandler SetPollingInterval(TimeSpan pollingInterval)
    //    {
    //        PollingInterval = pollingInterval;
    //        return this;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="seconds"></param>
    //    /// <returns></returns>
    //    public ElementHandler SetTimeoutSeconds(int seconds)
    //    {
    //        Timeout = TimeSpan.FromSeconds(seconds);
    //        return this;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="action"></param>
    //    public void PerformAction(Actions action)
    //    {
    //        if (action == null) throw new ArgumentNullException(nameof(action));
    //        action.Perform();
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void TakeScreenshot()
    //    {
    //        FileInfo file = new FileInfo(
    //            Path.Combine(Path.GetTempPath(),
    //            "screenshots",
    //            $"{((RemoteWebDriver)Driver).Capabilities.GetCapability("browserName")}_{DateTime.Now:yyyy.MM.dd_hh.mm.ss}.png"));
    //        Directory.CreateDirectory(file.DirectoryName);
    //        //TakeScreenshot(file);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public ElementHandler ClickElementWhenReady()
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = PollingInterval;
    //        wait.Timeout = Timeout;
    //        wait.IgnoreExceptionTypes(
    //            typeof(NoSuchElementException),
    //            typeof(InvalidElementStateException),
    //            typeof(ElementNotVisibleException),
    //            typeof(StaleElementReferenceException),
    //            typeof(ElementClickInterceptedException)
    //            );
    //        // Worth wrapping in a try/catch and returning false if not successful?
    //        wait.Until(d =>
    //        {
    //            d.FindElement(Locator).Click();
    //            return true;
    //        });
    //        return this;
    //    }

    //    /// <summary>
    //    /// Waits for the element to be in a valid state, then clicks on it.
    //    /// </summary>
    //    /// <param name="element">The IWebElement implementation to perform a Click on</param>
    //    /// <param name="secondsToTry">The seconds to wait for the element to be in a valid state before failing</param>
    //    [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
    //    public void ClickElementWhenReady(IWebElement element, int secondsToTry = 5)
    //    {
    //        WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
    //        wait.IgnoreExceptionTypes(
    //            typeof(ElementNotVisibleException),
    //            typeof(ElementNotInteractableException),
    //            typeof(StaleElementReferenceException),
    //            typeof(InvalidElementStateException),
    //            typeof(ElementClickInterceptedException),
    //            typeof(NoSuchElementException));

    //        // Worth wrapping in a try/catch and throwing inner exception?
    //        wait.Until(_ =>
    //        {
    //            element.Click();
    //            return true;
    //        });
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="locator"></param>
    //    /// <param name="textToSend"></param>
    //    /// <param name="secondsToTry"></param>
    //    public void SendKeysWhenReady(By locator, string textToSend, int secondsToTry = 5)
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
    //        wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
    //        wait.IgnoreExceptionTypes(
    //            typeof(NoSuchElementException),
    //            typeof(InvalidElementStateException),
    //            typeof(ElementNotVisibleException),
    //            typeof(StaleElementReferenceException),
    //            typeof(ElementNotInteractableException)
    //            );
    //        // Worth wrapping in a try/catch and returning false if not successful?
    //        wait.Until(d =>
    //        {
    //            IWebElement elem = d.FindElement(locator);
    //            elem.SendKeys(textToSend);
    //            return true;
    //        });
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="locator"></param>
    //    /// <param name="textToSend"></param>
    //    /// <param name="secondsToTry"></param>
    //    public void ReplaceText(By locator, string textToSend, int secondsToTry = 5)
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
    //        wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
    //        wait.IgnoreExceptionTypes(
    //            typeof(NoSuchElementException),
    //            typeof(InvalidElementStateException),
    //            typeof(ElementNotVisibleException),
    //            typeof(StaleElementReferenceException),
    //            typeof(ElementNotInteractableException)
    //            );
    //        // Worth wrapping in a try/catch and returning false if not successful?
    //        wait.Until(d =>
    //        {
    //            // NEED TO HANDLE KEYS.ENTER, KEYS.F1, ETC. HERE
    //            IWebElement elem = d.FindElement(locator);
    //            elem.Clear();
    //            elem.SendKeys(textToSend);
    //            System.Threading.Tasks.Task.Delay(100).Wait();
    //            return elem.GetAttribute("value") == textToSend;
    //            //return true;
    //        });
    //    }

    //    /// <summary>
    //    /// Waits for the element to be in a valid state, then performs a SendKeys to it.
    //    /// </summary>
    //    /// <param name="element">The IWebElement implementation to perform a SendKeys on</param>
    //    /// <param name="textToSend">The text to send to the element</param>
    //    /// <param name="secondsToTry">The seconds to wait for the element to be in a valid state before failing</param>
    //    [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
    //    public void SendKeysWhenReady(IWebElement element, string textToSend, int secondsToTry = 5)
    //    {
    //        WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
    //        wait.IgnoreExceptionTypes(
    //            typeof(ElementNotVisibleException),
    //            typeof(ElementNotInteractableException),
    //            typeof(StaleElementReferenceException),
    //            typeof(InvalidElementStateException),
    //            typeof(NoSuchElementException));

    //        wait.Until(_ => {
    //            element.Clear();
    //            element.SendKeys(textToSend);
    //            System.Threading.Tasks.Task.Delay(250).Wait();
    //            return element.GetAttribute("value") == textToSend;
    //        });
    //    }


    //    // TO DO: Fix new WaitForVisible and WaitForInvisible methods.
    //    // Currently if the elem.Displayed returns false, we retry always... 
    //    // which don't necessarily want to do depending on if we're waiting for the visible or invisible state

    //    /// <summary>
    //    /// Waits for the element to be visible.
    //    /// </summary>
    //    /// <param name="locator">The IWebElement implementation to check for visibility state</param>
    //    /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
    //    /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
    //    public bool WaitForVisibleState(By locator, int secondsToTry = 5)
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
    //        wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
    //        wait.IgnoreExceptionTypes(
    //            typeof(NoSuchElementException),
    //            typeof(StaleElementReferenceException)
    //            );

    //        try
    //        {
    //            return wait.Until(d => 
    //            {
    //                IWebElement elem = d.FindElement(locator);
    //                return elem.Displayed;
    //            });
    //        }
    //        catch (WebDriverTimeoutException ex)
    //            when (ex.InnerException is NoSuchElementException)
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Waits for the element to be visible.
    //    /// </summary>
    //    /// <param name="locator">The IWebElement implementation to check for visibility state</param>
    //    /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
    //    /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
    //    public bool WaitForInvisibleState(By locator, int secondsToTry = 5)
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
    //        wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
    //        wait.IgnoreExceptionTypes(
    //            typeof(StaleElementReferenceException)
    //            );

    //        try
    //        {
    //            return wait.Until(d =>
    //            {
    //                IWebElement elem = d.FindElement(locator);
    //                return !elem.Displayed;
    //            });
    //        }
    //        catch (NoSuchElementException)
    //        {
    //            return true;
    //        }
    //    }

    //    /// <summary>
    //    /// Waits for the element to be visible.
    //    /// </summary>
    //    /// <param name="element">The IWebElement implementation to check for visibility state</param>
    //    /// <param name="secondsToTry">The number of seconds to wait for the element to be visible before failing</param>
    //    /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
    //    [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
    //    public bool WaitForVisibleState(IWebElement element, int secondsToTry = 5)
    //    {
    //        WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
    //        wait.IgnoreExceptionTypes(
    //            typeof(ElementNotVisibleException),
    //            typeof(StaleElementReferenceException),
    //            typeof(NoSuchElementException));

    //        try
    //        {
    //            return wait.Until(_ => element.Displayed);
    //        }
    //        catch (WebDriverException ex)
    //            when (ex.InnerException is ElementNotVisibleException
    //                    || ex.InnerException is NoSuchElementException)
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Waits for the element to be enabled.
    //    /// </summary>
    //    /// <param name="locator">The IWebElement implementation to check for enabled state</param>
    //    /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
    //    /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
    //    public bool WaitForEnabledState(By locator, int secondsToTry = 5)
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
    //        wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
    //        wait.IgnoreExceptionTypes(
    //            typeof(NoSuchElementException),
    //            typeof(StaleElementReferenceException)
    //            );

    //        try
    //        {
    //            return wait.Until(d =>
    //            {
    //                IWebElement elem = d.FindElement(locator);
    //                return elem.Enabled;
    //            });
    //        }
    //        catch (WebDriverTimeoutException ex)
    //            when (ex.InnerException is NoSuchElementException)
    //        {
    //            return false;
    //        }
    //    }

    //    /// <summary>
    //    /// Waits for the element to be enabled.
    //    /// </summary>
    //    /// <param name="locator">The IWebElement implementation to check for enabled state</param>
    //    /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
    //    /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
    //    public bool WaitForDisabledState(By locator, int secondsToTry = 5)
    //    {
    //        DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(Driver);
    //        wait.PollingInterval = TimeSpan.FromMilliseconds(100);
    //        wait.Timeout = TimeSpan.FromSeconds(secondsToTry);
    //        wait.IgnoreExceptionTypes(
    //            typeof(NoSuchElementException),
    //            typeof(StaleElementReferenceException)
    //            );

    //        try
    //        {
    //            return wait.Until(d =>
    //            {
    //                IWebElement elem = d.FindElement(locator);
    //                return !elem.Enabled;
    //            });
    //        }
    //        catch (WebDriverTimeoutException ex)
    //            when (ex.InnerException is NoSuchElementException)
    //        {
    //            return true;
    //        }
    //    }

    //    /// <summary>
    //    /// Waits for the element to be enabled.
    //    /// </summary>
    //    /// <param name="element">The IWebElement implementation to check for enabled state</param>
    //    /// <param name="secondsToTry">The number of seconds to wait for the element to be enabled before failing</param>
    //    /// <returns>True if the element is visible, false if the the element is not visible or throws an ElementNotVisible or NoSuchElement exception</returns>
    //    [Obsolete("Obsoleting in favor of using methods taking in a By argument, to avoid nested waits.")]
    //    public bool WaitForEnabledState(IWebElement element, int secondsToTry = 5)
    //    {
    //        WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(secondsToTry));
    //        wait.IgnoreExceptionTypes(
    //            typeof(ElementNotInteractableException),
    //            typeof(StaleElementReferenceException),
    //            typeof(NoSuchElementException));

    //        try
    //        {
    //            return wait.Until(_ => element.Enabled);
    //        }
    //        catch(WebDriverException ex)
    //            when (ex.InnerException is ElementNotInteractableException)
    //        {
    //            return false;
    //        }
    //    }
    //}
}
