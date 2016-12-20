using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWpfApp.AutomatedUiTests.Windows;


namespace SimpleWpfApp.AutomatedUiTests.Tests
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class BaseTest
    {
        protected readonly SimpleWpfAppWindow SimpleAppWindow = new SimpleWpfAppWindow();

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            GenericPlaybackSettings.SetPlaybackSettings();
            SimpleWpfAppWindow.LaunchApplicationUnderTest();
        }

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}
    }
}
