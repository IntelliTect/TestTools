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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public DriverHandler Browser()
        //{
        //    return new DriverHandler(WrappedDriver);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="locator"></param>
        ///// <returns></returns>
        //public ElementHandler Element(By locator)
        //{
        //    return new ElementHandler(WrappedDriver, locator);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="locator"></param>
        ///// <returns></returns>
        //public ElementListHandler Elements(By locator)
        //{
        //    return new ElementListHandler(WrappedDriver, locator);
        //}
      
        //protected List<Type> ExceptionsToIgnore { get; } = new List<Type>();


        //public ElementHandler Element(By locator)
        //{
        //    Locator = locator;
        //    return this;
        //}



        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public DriverHandler Browser()
        //{
        //    return new DriverHandler(Driver);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="locator"></param>
        ///// <returns></returns>
        //public ElementHandler Element(By locator)
        //{
        //    return new ElementHandler(Driver, locator);
        //}
    }
}
