using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntelliTect.TestTools.Selenate.Examples.Pages
{
    public class DropDownPage
    {
        public DropDownPage(IWebDriver driver)
        {
            _Driver = driver;
        }

        public ElementHandler DropDown => new ElementHandler(_Driver, By.Id("dropdown"));
        public SelectElement DropDownSelect => new SelectElement(DropDown.FindElement());

        private IWebDriver _Driver;
    }
}
