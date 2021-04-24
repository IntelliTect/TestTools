using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliTect.TestTools.Selenate.Examples.Pages
{
    public class SliderPage
    {
        public SliderPage(IWebDriver driver)
        {
            _Driver = driver;
        }

        public ElementHandler Slider => new ElementHandler(_Driver, By.CssSelector("input[type='range']"));
        public ElementHandler Number => new ElementHandler(_Driver, By.Id("range"));

        private IWebDriver _Driver;
    }
}
