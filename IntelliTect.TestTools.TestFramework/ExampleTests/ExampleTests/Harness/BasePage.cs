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

        public string BaseUrl { get; set; } = @"https://intellitect.com/";
        [JsonIgnore]
        protected IWebDriver Driver { get; set; }
    }
}
