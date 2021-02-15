using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// 
    /// </summary>
    public class HandlerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public HandlerBase(IWebDriver driver)
        {
            if (driver is null) throw new ArgumentNullException(nameof(driver));
            WrappedDriver = driver;
        }

        /// <summary>
        /// 
        /// </summary>
        public IWebDriver WrappedDriver { get; }
        /// <summary>
        /// 
        /// </summary>
        protected TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 
        /// </summary>
        protected TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// 
        /// </summary>
        protected DefaultWait<IWebDriver> Wait => 
            new DefaultWait<IWebDriver>(WrappedDriver) 
            { 
                Timeout = Timeout, 
                PollingInterval = PollingInterval 
            };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeout"></param>
        /// <returns></returns>
        protected T SetTimeout<T>(TimeSpan timeout) where T : HandlerBase
        {
            if (timeout.TotalMilliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Please provide a positive value.");
            }

            Timeout = timeout;
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
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
