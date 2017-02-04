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
            Assert.AreEqual( NotePadWindowHelpers.ValidateTextFile(SaveLocation, "testNoCharacters.txt", Empty), "Unexpected text was found" );
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testNoCharacters.txt");
        }

        [TestMethod]
        [TestCategory("ExampleTest")]
        public void MinimumPossibleAlphaCharacters()
        {
            NotepadWindow.NotepadEdit.Text = AlphaMin;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testMinAlpha.txt");
            Assert.IsTrue(NotePadWindowHelpers.ValidateTextFile(SaveLocation, "testNoCharacters.txt", AlphaMin), "Unexpected text was found");
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testMinAlpha.txt");
        }

        [TestMethod]
        [TestCategory("ExampleTest")]
        public void MinimumPossibleNumericCharacters()
        {
            NotepadWindow.NotepadEdit.Text = NumericMin;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testMinNumeric.txt");
            Assert.IsTrue(NotePadWindowHelpers.ValidateTextFile(SaveLocation, "testNoCharacters.txt", NumericMin), "Unexpected text was found");
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testMinNumeric.txt");
        }
    }
}