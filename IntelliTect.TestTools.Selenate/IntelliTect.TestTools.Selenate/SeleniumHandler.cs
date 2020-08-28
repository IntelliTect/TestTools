using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliTect.TestTools.Selenate
{
    /// <summary>
    /// 
    /// </summary>
    public class SeleniumHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        public SeleniumHandler(IWebDriver driver)
        {
            WrappedDriver = driver;
            Driver = new DriverHandler(WrappedDriver);
            Element = new ElementHandler(WrappedDriver);
            Elements = new ElementsHandler(WrappedDriver);
        }

        private IWebDriver WrappedDriver { get; }

        /// <summary>
        /// 
        /// </summary>
        public DriverHandler Driver { get; }
        /// <summary>
        /// 
        /// </summary>
        public ElementHandler Element { get; }
        /// <summary>
        /// 
        /// </summary>
        public ElementsHandler Elements { get; }
    }
}
