using OpenQA.Selenium;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Examples
{
    public class BasicDriverInteractions : IDisposable
    {
        public BasicDriverInteractions()
        {
            _WebDriver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
            _Driver = new DriverHandler(_WebDriver);
        }

        private readonly IWebDriver _WebDriver;
        private readonly DriverHandler _Driver;

        [Fact]
        public void NavigateAndGetWindowTitle()
        {
            _Driver.NavigateToPage("http://the-internet.herokuapp.com/");

            Assert.Equal("The Internet", _Driver.GetWindowTitle());
        }

        [Fact]
        public void Click()
        {
            _Driver.NavigateToPage("http://the-internet.herokuapp.com/")
                .FindElement(By.CssSelector("a[href='/abtest']"))
                .Click();

            Assert.Equal("A/B Test Control", _Driver.FindElement(By.CssSelector("div[class='example']>h3"))
                .GetElementText());
        }

        [Fact]
        public void FindAlert()
        {
            _Driver.NavigateToPage("http://the-internet.herokuapp.com/javascript_alerts")
                .FindElement(By.CssSelector("button[onclick='jsConfirm()']"))
                .Click();

            _Driver.SwitchToAlert().Accept();

            Assert.Equal(
                "You clicked: Ok",
                _Driver.FindElement(By.CssSelector("p[id='result']")).GetElementText());
        }

        [Fact]
        public void FindWindow()
        {
            _Driver.NavigateToPage("http://the-internet.herokuapp.com/windows")
                .FindElement(By.CssSelector("a[href='/windows/new']"))
                .Click();

            // If the window is not found, this will throw
            Assert.Equal("New Window",
                _Driver
                .SwitchToWindow("New Window")
                .GetWindowTitle());
        }

        [Fact]
        public void FindIFrame()
        {
            _Driver.NavigateToPage("http://the-internet.herokuapp.com/nested_frames");

            Assert.Equal("LEFT",
                _Driver.SwitchToIFrame(
                    By.CssSelector("frame[src='/frame_top']"),
                    By.CssSelector("frame[src='/frame_left']"))
                .FindElement(By.CssSelector("body"))
                .GetElementText());
        }

        public void Dispose()
        {
            _WebDriver.Dispose();
        }
    }
}
