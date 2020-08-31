using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// 
    /// </summary>
    public class ElementsHandler : HandlerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public ElementsHandler(IWebDriver driver, By locator) : base(driver)
        {
            //Driver = driver;
            Locator = locator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public ElementsHandler(IWebDriver driver) : base(driver) { }

        private By Locator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public ElementsHandler SetTimeout(TimeSpan timeout)
        {
            return SetTimeout<ElementsHandler>(timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        /// <returns></returns>
        public ElementsHandler SetTimeoutSeconds(int timeoutInSeconds)
        {
            return SetTimeout<ElementsHandler>(TimeSpan.FromSeconds(timeoutInSeconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
        public ElementsHandler SetPollingInterval(TimeSpan pollingInterval)
        {
            return SetPollingInterval<ElementsHandler>(pollingInterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollIntervalInMilliseconds"></param>
        /// <returns></returns>
        public ElementsHandler SetPollingIntervalMilliseconds(int pollIntervalInMilliseconds)
        {
            return SetPollingInterval<ElementsHandler>(TimeSpan.FromMilliseconds(pollIntervalInMilliseconds));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public bool ContainsText(string text)
        {
            IWait<IWebDriver> wait = Wait;
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            try
            {
                return wait.Until(d => d.FindElements(Locator).Any(h => h.Text == text));
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}
