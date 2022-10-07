namespace IntelliTect.TestTools.Selenate.Examples;

public class TestBase : IDisposable
{
    private bool _DisposedValue;

    public TestBase()
    {
        WebDriver = new WebDriverFactory(BrowserType.Chrome).GetDriver();
        DriverHandler = new DriverHandler(WebDriver);
    }

    protected IWebDriver WebDriver { get; }
    protected DriverHandler DriverHandler { get; }

    protected virtual void Dispose(bool disposing)
    {
        if (!_DisposedValue)
        {
            if (disposing)
            {
                WebDriver.Dispose();
            }
            _DisposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}
