namespace IntelliTect.TestTools.Selenate.Examples.Pages;

public class DynamicLoadingPages
{
    public DynamicLoadingPages(IWebDriver driver)
    {
        _Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    // Multiple ways to approach element handler instantiation...
    // This will reset any settings every time a new reference is made, so polling interval, timeout, etc. will not be retained across multiple calls within the same test.
    public ElementHandler StartButton => new ElementHandler(_Driver, By.CssSelector("div[id='start']>button"));

    // This will retain any settings modified on the object across multiple calls within the same test, e.g. polling interval or timeout
    public ElementHandler HelloWorldLabel
    {
        get
        {
            _HelloWorldLabel ??= new ElementHandler(_Driver, By.CssSelector("div[id='finish']>h4"));
            return _HelloWorldLabel;
        }
    }

    private ElementHandler? _HelloWorldLabel;
    private IWebDriver _Driver;
}
