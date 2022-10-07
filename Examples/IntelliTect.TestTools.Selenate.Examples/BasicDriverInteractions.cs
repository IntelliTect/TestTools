namespace IntelliTect.TestTools.Selenate.Examples;

public class BasicDriverInteractions : TestBase
{
    [Fact]
    public void NavigateAndGetWindowTitle()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/");

        Assert.Equal("The Internet", DriverHandler.GetWindowTitle());
    }

    [Fact]
    public void FindAlert()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/javascript_alerts")
            .FindElement(By.CssSelector("button[onclick='jsConfirm()']"))
            .Click();

        DriverHandler.SwitchToAlert().Accept();

        Assert.Equal(
            "You clicked: Ok",
            DriverHandler.FindElement(By.CssSelector("p[id='result']")).Text());
    }

    [Fact]
    public void FindWindow()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/windows")
            .FindElement(By.CssSelector("a[href='/windows/new']"))
            .Click();

        // If the window is not found, this will throw
        Assert.Equal("New Window",
            DriverHandler
            .SwitchToWindow("New Window")
            .GetWindowTitle());
    }

    [Fact]
    public void FindFrame()
    {
        DriverHandler.NavigateToPage("https://the-internet.herokuapp.com/nested_frames");

        Assert.Equal("LEFT",
            DriverHandler.SwitchToIFrame(
                By.CssSelector("frame[src='/frame_top']"),
                By.CssSelector("frame[src='/frame_left']"))
            .FindElement(By.CssSelector("body"))
            .Text());
    }
}
