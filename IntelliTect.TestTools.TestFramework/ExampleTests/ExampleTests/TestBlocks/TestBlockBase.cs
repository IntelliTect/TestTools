using ExampleTests.Pages;
using OpenQA.Selenium;

namespace ExampleTests.TestBlocks
{
    // Better to do this as constructor, or straight property?
    // Would be less code for user if it was a property... No constructors calling back to base
    public class TestBlockBase
    {
        public TestBlockBase(IWebDriver driver)
        {
            Driver = driver;
            HomePage = new HomePage(Driver);
            Blogs = new Blogs(Driver);
        }

        public HomePage HomePage { get; set; }
        public Blogs Blogs { get; set; }
        protected IWebDriver Driver { get; set; }
    }
}
