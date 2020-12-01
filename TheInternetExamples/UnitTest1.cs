using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Xunit;

namespace TheInternetExamples
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            IWebDriver test = new ChromeDriver();
            test.Url = "http://the-internet.herokuapp.com/";
        }
    }
}
