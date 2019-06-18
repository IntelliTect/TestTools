using ExampleTests.Pages;
using IntelliTect.TestTools.TestFramework;
using Newtonsoft.Json;
using OpenQA.Selenium;

namespace ExampleTests.TestBlocks
{
    // Better to do this as constructor, or straight property?
    // Would be less code for user if it was a property... No constructors calling back to base
    public class TestBlockBase : ITestBlock
    {
        //public TestBlockBase(IWebDriver driver)
        //{
        //    Driver = driver;
        //    HomePage = new HomePage(Driver);
        //    Blogs = new Blogs(Driver);
        //}

        public Pages.IntelliTect IntelliTect { get; set; }

        //[JsonIgnore]
        //public HomePage HomePage { get; set; }
        //[JsonIgnore]
        //public Blogs Blogs { get; set; }
        //[JsonIgnore]
        //public IWebDriver Driver { get; set; }
    }
}
