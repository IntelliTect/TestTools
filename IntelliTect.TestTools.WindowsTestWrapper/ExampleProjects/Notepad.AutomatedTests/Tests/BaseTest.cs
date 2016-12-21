using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notepad.AutomatedTests.Helpers;
using Notepad.AutomatedTests.Windows;

namespace Notepad.AutomatedTests.Tests
{
    /// <summary>
    /// Summary description for BaseTest
    /// </summary>
    [CodedUITest]
    public partial class BaseTest
    {
        protected readonly NotepadWindow NotepadWindow = new NotepadWindow();
        protected readonly SaveAsWindow SaveAsWindow = new SaveAsWindow();
        protected readonly NotepadWindowHelpers NotePadWindowHelpers = new NotepadWindowHelpers();
        protected readonly Utilities Utilities = new Utilities();

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            GenericPlaybackSettings.SetPlaybackSettings();
            NotepadWindow.LaunchApplicationUnderTest();
        }

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
    }
}