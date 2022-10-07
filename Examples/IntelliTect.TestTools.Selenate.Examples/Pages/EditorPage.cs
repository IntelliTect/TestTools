using System;

namespace IntelliTect.TestTools.Selenate.Examples.Pages;

public class EditorPage
{
    public EditorPage(IWebDriver driver)
    {
        _Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    public ElementHandler MenuBar => new ElementHandler(_Driver, By.CssSelector("div[role='menubar']"));
    public ElementHandler FormatMenu => new ElementHandler(_Driver, By.XPath("//span[text() = 'Format']"));
    public ElementHandler FormatsMenu => new ElementHandler(_Driver, By.XPath("//div[text() = 'Formats']"));
    public ElementHandler HeadingsMenu => new ElementHandler(_Driver, By.XPath("//div[text() = 'Headings']"));
    public ElementHandler Heading1Option => new ElementHandler(_Driver, By.XPath("//h1[text() = 'Heading 1']"));
    public ElementHandler ParagraphDropDown => new ElementHandler(_Driver, By.CssSelector("span[class='tox-tbtn__select-label']"));

    private IWebDriver _Driver;
}
