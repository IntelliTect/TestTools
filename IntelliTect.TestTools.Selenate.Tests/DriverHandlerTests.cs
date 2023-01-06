using System.Reflection;

namespace IntelliTect.TestTools.Selenate.Tests;

public class DriverHandlerTests
{
    [Fact]
    public void SetTimeoutWithNegativeValueThrowsException()
    {
        var mockDriver = new Mock<IWebDriver>();
        DriverHandler handler = new(mockDriver.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() => handler.SetTimeout(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void SetTimeoutSecondsWithNegativeValueThrowsException()
    {
        var mockDriver = new Mock<IWebDriver>();
        DriverHandler handler = new(mockDriver.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() => handler.SetTimeoutSeconds(-1));
    }

    [Fact]
    public void SetPollingIntervalWithNegativeValueThrowsException()
    {
        var mockDriver = new Mock<IWebDriver>();
        DriverHandler handler = new(mockDriver.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() => handler.SetPollingInterval(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void SetPollingIntervalMillisecondsWithNegativeValueThrowsException()
    {
        var mockDriver = new Mock<IWebDriver>();
        DriverHandler handler = new(mockDriver.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() => handler.SetPollingIntervalMilliseconds(-1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void NavigateToPageWithEmptyStringThrowsException(string val)
    {
        var mockDriver = new Mock<IWebDriver>();
        DriverHandler handler = new(mockDriver.Object);

        Assert.Throws<ArgumentNullException>(() => handler.NavigateToPage(val));
    }

    [Fact]
    public void SetTimeoutChangesDefaultValue()
    {
        var mockDriver = new Mock<IWebDriver>();

        DriverHandler handler = new(mockDriver.Object);
        handler.SetTimeout(TimeSpan.FromSeconds(1));

        Assert.Equal(
            TimeSpan.FromSeconds(1),
            handler
                .GetType()
                .GetProperty("Timeout", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(handler));
    }

    [Fact]
    public void SetTimeoutSecondsChangesDefaultValue()
    {
        var mockDriver = new Mock<IWebDriver>();

        DriverHandler handler = new(mockDriver.Object);
        handler.SetTimeoutSeconds(1);

        Assert.Equal(
            TimeSpan.FromSeconds(1),
            handler
                .GetType()
                .GetProperty("Timeout", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(handler));
    }

    [Fact]
    public void SetPollingIntervalChangesDefaultValue()
    {
        var mockDriver = new Mock<IWebDriver>();

        DriverHandler handler = new(mockDriver.Object);
        handler.SetPollingInterval(TimeSpan.FromMilliseconds(1));

        Assert.Equal(
            TimeSpan.FromMilliseconds(1),
            handler.GetType().GetProperty("PollingInterval", BindingFlags.Instance | BindingFlags.NonPublic)?
            .GetValue(handler));
    }

    [Fact]
    public void SetPollingMillisecondsIntervalChangesDefaultValue()
    {
        var mockDriver = new Mock<IWebDriver>();

        DriverHandler handler = new(mockDriver.Object);
        handler.SetPollingIntervalMilliseconds(1);

        Assert.Equal(
            TimeSpan.FromMilliseconds(1),
            handler
                .GetType()
                .GetProperty("PollingInterval", BindingFlags.Instance | BindingFlags.NonPublic)?
                .GetValue(handler));
    }

    [Fact]
    public void NavigateToPageProperlySetsWebDriverUrl()
    {
        var mockNavigation = new Mock<INavigation>();
        mockNavigation
            .Setup(n => n.GoToUrl(It.IsNotNull<Uri>()))
            .Verifiable();

        var mockDriver = new Mock<IWebDriver>();
        mockDriver
            .Setup(x => x.Navigate())
            .Returns(mockNavigation.Object);

        DriverHandler handler = new(mockDriver.Object);
        handler.NavigateToPage(new Uri("http://www.someSuccess.com/"));

        mockNavigation.Verify(d => d.GoToUrl(It.IsNotNull<Uri>()), Times.Once);
    }

    [Fact]
    public void NavigateToPageWithStringProperlySetsWebDriverUrl()
    {
        var mockNavigation = new Mock<INavigation>();
        mockNavigation
            .Setup(n => n.GoToUrl(It.IsNotNull<Uri>()))
            .Verifiable();

        var mockDriver = new Mock<IWebDriver>();
        mockDriver.Setup(x => x.Navigate()).Returns(mockNavigation.Object);

        DriverHandler handler = new(mockDriver.Object);
        handler.NavigateToPage("http://www.someSuccess.com/");

        mockNavigation.Verify(d => d.GoToUrl(It.IsNotNull<Uri>()), Times.Once);
    }

    [Fact]
    public void FindElementReturnsElementHandler()
    {
        var mockElement = new Mock<IWebElement>();
        var mockDriver = new Mock<IWebDriver>();
        mockDriver
            .Setup(x => x.FindElement(It.IsAny<By>()))
            .Returns(mockElement.Object);

        DriverHandler handler = new(mockDriver.Object);
        ElementHandler elem = handler.FindElement(By.Id("Testing"));
        Assert.NotNull(elem);
    }

    [Fact]
    public void FindElementsReturnsElementsHandler()
    {
        var mockElement = new Mock<IWebElement>();
        var mockDriver = new Mock<IWebDriver>();
        mockDriver
            .Setup(x => x.FindElements(It.IsAny<By>()))
            .Returns(new ReadOnlyCollection<IWebElement>(
                new List<IWebElement> { mockElement.Object }));

        DriverHandler handler = new(mockDriver.Object);
        ElementsHandler elem = handler.FindElements(By.Id("Testing"));
        Assert.NotNull(elem);
    }

    [Fact]
    public void GetWindowTitleReturnsDriverProperty()
    {
        string testTitle = "Test Window Title";
        var mockDriver = new Mock<IWebDriver>();
        mockDriver
            .SetupGet(w => w.Title)
            .Returns(testTitle);

        DriverHandler handler = new(mockDriver.Object);

        string title = handler.GetWindowTitle();

        Assert.Equal(testTitle, title);
    }

    [Fact]
    public void SwitchWindowInvokesSwitchToWindow()
    {
        string windowTitle = "Testing!";
        var mockDriver = new Mock<IWebDriver>();

        var mockNavigation = new Mock<ITargetLocator>();
        mockNavigation
            .Setup(n => n.Window(It.IsAny<string>()))
            .Returns(mockDriver.Object)
            .Verifiable();

        mockDriver
            .Setup(w => w.SwitchTo())
            .Returns(mockNavigation.Object);

        mockDriver
            .Setup(w => w.Title)
            .Returns(windowTitle);
        mockDriver
            .Setup(h => h.WindowHandles)
            .Returns(new ReadOnlyCollection<string>(new List<string> { windowTitle }));


        DriverHandler handler = new(mockDriver.Object);
        handler.SwitchToWindow(windowTitle);

        mockNavigation.Verify(w => w.Window(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void SwitchAlertInvokesSwitchToAlert()
    {
        var mockAlert = new Mock<IAlert>();

        var mockNavigation = new Mock<ITargetLocator>();
        mockNavigation
            .Setup(n => n.Alert())
            .Returns(mockAlert.Object)
            .Verifiable();

        var mockDriver = new Mock<IWebDriver>();
        mockDriver
            .Setup(w => w.SwitchTo())
            .Returns(mockNavigation.Object);

        DriverHandler handler = new(mockDriver.Object);
        handler.SwitchToAlert();

        mockNavigation.Verify(w => w.Alert(), Times.Once);
    }

    [Fact]
    public void SwitchFrameInvokesSwitchToFrame()
    {
        var mockElement = new Mock<IWebElement>();
        var mockDriver = new Mock<IWebDriver>();
        mockDriver
            .Setup(x => x.FindElement(It.IsAny<By>()))
            .Returns(mockElement.Object);

        var mockNavigation = new Mock<ITargetLocator>();
        mockNavigation
            .Setup(n => n.Frame(It.IsAny<IWebElement>()))
            .Returns(mockDriver.Object)
            .Verifiable();

        mockDriver
            .Setup(w => w.SwitchTo())
            .Returns(mockNavigation.Object);

        DriverHandler handler = new(mockDriver.Object);
        handler.SwitchToIFrame(By.Id("Testing!"));

        mockNavigation.Verify(w => w.Frame(It.IsAny<IWebElement>()), Times.Once);
    }
}
