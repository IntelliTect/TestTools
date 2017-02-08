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
            NotePadWindowHelpers.DeleteDocument( SaveLocation, "testNoCharacters.txt");
            NotepadWindow.NotepadEdit.Text = Empty;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testNoCharacters.txt");
            //Assert.AreEqual( NotePadWindowHelpers.CheckTextFileForExpectedText(SaveLocation, "testNoCharacters.txt", Empty), "Unexpected text was found" );
            Assert.IsTrue( NotePadWindowHelpers.CheckTextFileForExpectedText( SaveLocation, "testNoCharacters.txt", Empty ) );
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testNoCharacters.txt");
        }

        [TestMethod]
        [TestCategory("ExampleTest")]
        public void MinimumPossibleAlphaCharacters()
        {
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testMinAlpha.txt");
            NotepadWindow.NotepadEdit.Text = AlphaMin;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testMinAlpha.txt");
            //Assert.IsTrue(NotePadWindowHelpers.CheckTextFileForExpectedText(SaveLocation, "testNoCharacters.txt", AlphaMin), "Unexpected text was found");
            Assert.IsTrue(NotePadWindowHelpers.CheckTextFileForExpectedText(SaveLocation, "testMinAlpha.txt", AlphaMin));
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testMinAlpha.txt");
        }

        [TestMethod]
        [TestCategory("ExampleTest")]
        public void MinimumPossibleNumericCharacters()
        {
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testMinNumeric.txt");
            NotepadWindow.NotepadEdit.Text = NumericMin;
            NotePadWindowHelpers.SaveDocument(SaveLocation, "testMinNumeric.txt");
            //Assert.IsTrue(NotePadWindowHelpers.CheckTextFileForExpectedText(SaveLocation, "testNoCharacters.txt", NumericMin), "Unexpected text was found");
            Assert.IsTrue(NotePadWindowHelpers.CheckTextFileForExpectedText(SaveLocation, "testMinNumeric.txt", NumericMin));
            NotePadWindowHelpers.DeleteDocument(SaveLocation, "testMinNumeric.txt");
        }
    }
}