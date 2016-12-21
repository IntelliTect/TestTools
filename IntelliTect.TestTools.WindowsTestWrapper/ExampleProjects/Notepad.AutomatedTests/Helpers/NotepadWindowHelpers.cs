using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UITesting;
using Notepad.AutomatedTests.Windows;

namespace Notepad.AutomatedTests.Helpers
{
    public class NotepadWindowHelpers
    {
        private readonly NotepadWindow _NotepadWindow = new NotepadWindow();
        private readonly SaveAsWindow _SaveAsWindow = new SaveAsWindow();

        //TODO: Is there a better way to handle saving a document for a test?
        public void SaveDocument(string location, string filename)
        {
            //Check if doc with same name already exists in location before saving
            string formattedPath = Path.Combine(location, filename);
            if (File.Exists(formattedPath))
            {
                //TODO: Fix this so it appends to the name BEFORE the extension, per Kevin's suggestion
                filename += "1";
            }
            Mouse.Click();
            Mouse.Click(_NotepadWindow.Save);
            Mouse.Click(_SaveAsWindow.FindSaveLocationToolbar);
            _SaveAsWindow.SaveAsBox.EditableItem = filename;
            _SaveAsWindow.SaveButton.SetFocus();
            SendKeys.SendWait("{ENTER}");
        }
    }
}