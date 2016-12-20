using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UITesting;
using Notepad.AutomatedTests.Windows;

namespace Notepad.AutomatedTests.Helpers
{
    public class NotepadWindowHelpers
    {
        readonly NotepadWindow _NotepadWindow = new NotepadWindow();
        readonly SaveAsWindow _SaveAsWindow = new SaveAsWindow();

        //Ordinarily this would contain more complex operations.
        //Use this to abstract away some of the complexity to help the test author.
        public void SaveDocument(string location, string filename)
        {
            //Check if doc with same name already exists in location before saving
            string formattedPath = location + "\\" + filename;
            if (File.Exists(formattedPath))
            {
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