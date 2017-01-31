using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Notepad.AutomatedTests.Tests
{
    [CodedUITest]
    public class ExampleNotepadTest : BaseTest
    {
        //These may be turned into data drive tests for conciseness and flexibility
        [TestMethod]
        [TestCategory("ExampleTest")]
        public void NoCharactersEntered()
        {
            NotepadWindow.NotepadEdit.Text = Empty;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testNoCharacters.txt");
            var text = Utilities.ParseTextFile(SaveLocation + @"\testNoCharacters.txt");
            Assert.AreEqual(Empty, text, "Unexpected text was found");
            Utilities.DeleteDocument(SaveLocation, "testNoCharacters.txt");
        }

        [TestMethod]
        [TestCategory("ExampleTest")]
        public void MinimumPossibleAlphaCharacters()
        {
            NotepadWindow.NotepadEdit.Text = AlphaMin;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testMinAlpha.txt");
            var text = Utilities.ParseTextFile(SaveLocation + @"\testMinAlpha.txt");
            Assert.AreEqual(AlphaMin, text, "Unexpected text was found");
            Utilities.DeleteDocument(SaveLocation, "testMinAlpha.txt");
        }

        [TestMethod]
        [TestCategory("ExampleTest")]
        public void MinimumPossibleNumericCharacters()
        {
            NotepadWindow.NotepadEdit.Text = NumericMin;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testMinNumeric.txt");
            var text = Utilities.ParseTextFile(SaveLocation + @"\testMinNumeric.txt");
            Assert.AreEqual(NumericMin, text, "Unexpected text was found");
            Utilities.DeleteDocument(SaveLocation, "testMinNumeric.txt");
        }
    }
}