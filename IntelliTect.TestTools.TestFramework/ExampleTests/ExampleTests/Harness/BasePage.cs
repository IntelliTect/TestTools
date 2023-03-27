using System;
using System.Text.Json.Serialization;
using OpenQA.Selenium;

namespace ExampleTests.Harness
{
    public class BasePage
    {
        public BasePage(IWebDriver driver)
        {
            Driver = driver;
        }

        public Uri BaseUrl { get; set; } = new Uri("https://intellitect.com/");
        [JsonIgnore]
        protected IWebDriver Driver { get; set; }
    }
}
