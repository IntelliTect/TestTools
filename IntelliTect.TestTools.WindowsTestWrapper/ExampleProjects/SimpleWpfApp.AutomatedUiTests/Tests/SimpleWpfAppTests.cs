using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SimpleWpfApp.AutomatedUiTests.Windows.SimpleWpfAppWindow;
namespace SimpleWpfApp.AutomatedUiTests.Tests
{
    [CodedUITest]
    public class SimpleWpfAppTests : BaseTest
    {
        [TestMethod]
        public void AddSingleListItemAndVerify()
        {
            SimpleAppWindow.ListEntry.Text = "Test";
            Mouse.Click(SimpleAppWindow.SaveButton);
            Mouse.Click(SimpleAppWindow.ExpandListItem);
            Assert.IsTrue( SimpleAppWindow.CheckAllListItems( "Test" ), "An unexpected list item was found" );
        }

        [TestMethod]
        public void AddMultipleListItemAndVerify()
        {
            SimpleAppWindow.ListEntry.Text = "Test";
            Mouse.Click(SimpleAppWindow.SaveButton);
            SimpleAppWindow.ListEntry.Text = "Test2";
            Mouse.Click(SimpleAppWindow.SaveButton);
            Mouse.Click(SimpleAppWindow.ExpandListItem);
            Assert.IsTrue(SimpleAppWindow.CheckAllListItems("Test", "Test2"), "An unexpected list item was found");
        }

        [TestMethod]
        public void AddEmptyListItem()
        {
            Mouse.Click(SimpleAppWindow.SaveButton);
            Mouse.Click(SimpleAppWindow.ExpandListItem);
            Assert.IsTrue( SimpleAppWindow.CheckAllListItems( "" ), "An unexpected list item was found");
        }

        [TestMethod]
        public void AddNoListItem()
        {
            Mouse.Click(SimpleAppWindow.ExpandListItem);
            Assert.IsTrue(SimpleAppWindow.CheckAllListItems(), "An unexpected list item was found");
        }

        [TestMethod]
        public void SaveByFindSibling()
        {
            SimpleAppWindow.ListEntry.Text = "Test";
            Mouse.Click(SimpleAppWindow.SaveButtonAlternateFind);
            Mouse.Click(SimpleAppWindow.ExpandListItem);
            Assert.IsTrue( SimpleAppWindow.CheckAllListItems( "Test" ), "The text was not saved when finding the Save button by sibling" );
        }
    }
}