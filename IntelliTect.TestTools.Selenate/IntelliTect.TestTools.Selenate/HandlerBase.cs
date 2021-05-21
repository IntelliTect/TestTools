using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// Base class for handling Selenium interactions.
    /// </summary>
    public class HandlerBase
    {
        /// <summary>
        /// Base class for handling Selenium interactions.
        /// </summary>
        /// <param name="driver">The WebDriver needed to driver all of the Selenium interactions</param>
        public HandlerBase(IWebDriver driver)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            WrappedDriver = driver;
        }

        /// <summary>
        /// The WebDriver this instance is wrapping.
        /// </summary>
        public IWebDriver WrappedDriver { get; }
        protected TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);
        protected TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);
        protected DefaultWait<IWebDriver> Wait => 
            new(WrappedDriver) 
            { 
                Timeout = Timeout, 
                PollingInterval = PollingInterval 
            };

        protected T SetTimeout<T>(TimeSpan timeout) where T : HandlerBase
        {
            if (timeout.TotalMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Please provide a positive value.");
            }

            Timeout = timeout;
            return (T)this;
        }

        protected T SetPollingInterval<T>(TimeSpan pollingInterval) where T : HandlerBase
        {
            if (pollingInterval.TotalMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pollingInterval), "Please provide a positive value.");
            }

            PollingInterval = pollingInterval;
            return (T)this;
        }
    }
}
