using OpenQA.Selenium;
using System;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Examples
{
    public class BasicDriverInteractions : TestBase
    {
        [Fact]
        public void NavigateAndGetWindowTitle()
        {
            _DriverHandler.NavigateToPage("http://the-internet.herokuapp.com/");

            Assert.Equal("The Internet", _DriverHandler.GetWindowTitle());
        }

        [Fact]
        public void Click()
        {
            _DriverHandler.NavigateToPage("http://the-internet.herokuapp.com/")
                .FindElement(By.CssSelector("a[href='/abtest']"))
                .Click();

            Assert.Equal("A/B Test Control", _DriverHandler.FindElement(By.CssSelector("div[class='example']>h3"))
                .GetElementText());
        }

        [Fact]
        public void FindAlert()
        {
            _DriverHandler.NavigateToPage("http://the-internet.herokuapp.com/javascript_alerts")
                .FindElement(By.CssSelector("button[onclick='jsConfirm()']"))
                .Click();

            _DriverHandler.SwitchToAlert().Accept();

            Assert.Equal(
                "You clicked: Ok",
                _DriverHandler.FindElement(By.CssSelector("p[id='result']")).GetElementText());
        }

        [Fact]
        public void FindWindow()
        {
            _DriverHandler.NavigateToPage("http://the-internet.herokuapp.com/windows")
                .FindElement(By.CssSelector("a[href='/windows/new']"))
                .Click();

            // If the window is not found, this will throw
            Assert.Equal("New Window",
                _DriverHandler
                .SwitchToWindow("New Window")
                .GetWindowTitle());
        }

        [Fact]
        public void FindFrame()
        {
            _DriverHandler.NavigateToPage("http://the-internet.herokuapp.com/nested_frames");

            Assert.Equal("LEFT",
                _DriverHandler.SwitchToIFrame(
                    By.CssSelector("frame[src='/frame_top']"),
                    By.CssSelector("frame[src='/frame_left']"))
                .FindElement(By.CssSelector("body"))
                .GetElementText());
        }
    }
}
