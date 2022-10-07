namespace IntelliTect.TestTools.Selenate.Tests;

public class ElementsHandlerTests
{
    [Fact]
    public void GetTextReturnsExpectedWhenFound()
    {
        Assert.True(SetupMockedData().ContainsText("Testing1"));
    }

    [Fact]
    public void GetTextReturnsFalseWhenUnableToFindElementWithText()
    {
        Assert.False(SetupMockedData().ContainsText("TestingA"));
    }

    [Fact]
    public void GetSpecificExistingElementReturnsFoundElements()
    {
        Assert.NotNull(
            SetupMockedData()
            .GetSingleWebElement(x => 
                x.Displayed));
    }

    [Fact]
    public void GetSpecificExistingElementThrowsWhenNoElementsMatch()
    {
        Assert.Throws<WebDriverTimeoutException>(() => 
            SetupMockedData()
            .GetSingleWebElement(x => 
                x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetElementsThrowsWhenNoElementsMatch()
    {
        Assert.Throws<WebDriverTimeoutException>(() =>
            SetupMockedData()
            .GetAllWebElements(x =>
                x.Text.Contains("Blaaaargh", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetSpecificExistingElementThrowsWhenMultipleElementsMatch()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            SetupMockedData()
            .GetSingleWebElement(x =>
                x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetElementsReturnsWhenMultipleElementsMatch()
    {
        Assert.Equal(2,
            SetupMockedData()
            .GetAllWebElements(x =>
                x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase))
            .Count);
    }

    [Fact]
    public void GetSpecificExistingElementThrowsWhenNoElementsAreFound()
    {
        Assert.Throws<WebDriverTimeoutException>(() =>
            SetupMockedData()
            .SetLocator(By.Id("blarg"))
            .GetSingleWebElement(x =>
                x.Text.Contains("Testing", StringComparison.OrdinalIgnoreCase)));
    }

    private static ElementsHandler SetupMockedData()
    {
        var mockElement1 = new Mock<IWebElement>();
        mockElement1.SetupGet(e1 => e1.Text).Returns("Testing1");
        mockElement1.SetupGet(e1 => e1.Displayed).Returns(true);
        
        var mockElement2 = new Mock<IWebElement>();
        mockElement2.SetupGet(e2 => e2.Text).Returns("Testing2");
        mockElement2.SetupGet(e2 => e2.Displayed).Returns(false);
        var mockDriver = new Mock<IWebDriver>();
        mockDriver.Setup
            (f => f.FindElements(By.Id("test")))
            .Returns(
                new ReadOnlyCollection<IWebElement>(
                    new List<IWebElement> { mockElement1.Object, mockElement2.Object }));

        mockDriver.Setup
            (f => f.FindElements(By.Id("blarg")))
            .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>()));

        return new ElementsHandler(mockDriver.Object, By.Id("test"))
            .SetTimeout(TimeSpan.FromMilliseconds(20))
            .SetPollingIntervalMilliseconds(10);
    }
}