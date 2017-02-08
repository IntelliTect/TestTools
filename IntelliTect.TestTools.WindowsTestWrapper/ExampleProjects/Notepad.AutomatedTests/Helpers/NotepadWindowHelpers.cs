using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UITesting;
using Notepad.AutomatedTests.Windows;

namespace Notepad.AutomatedTests.Helpers
{
    public class NotepadWindowHelpers
    {
        private readonly NotepadWindow _NotepadWindow = new NotepadWindow();
        private readonly SaveAsWindow _SaveAsWindow = new SaveAsWindow();

        public bool CheckTextFileForExpectedText(string location, string fileName, string expectedText)
        {
            //Thread.Sleep( 100 );    //Wait just in case the file is being saved. If this ever fails, write a polling loop
            //return File.ReadAllText( Path.Combine( location, fileName ) ) == expectedText;

            bool result = false;
            string combinedPath = Path.Combine(location, fileName);
            int retryCount = 0;
            int numberOfRetries = 10;

            do
            {
                try
                {
                    if ( File.ReadAllText( combinedPath ) == expectedText )
                    {
                        result = true;
                        break;
                    }
                    if ( File.ReadAllText( combinedPath ) != expectedText )
                    {
                        Debug.WriteLine( "Text did not match" );
                        Debug.Write( $"Starting text: {combinedPath}" );
                        Debug.Write( $"Ending text: {expectedText}" );
                        result = false;
                        break;
                    }
                }
                //Catch just in case the file has not yet been saved
                catch ( FileNotFoundException )
                {
                    if (retryCount == numberOfRetries)
                    {
                        Debug.WriteLine($"Could not read file after {numberOfRetries} attempts.");
                        throw;
                    }
                }
                retryCount++;
                Thread.Sleep(100);
            } while (retryCount < numberOfRetries);


            return result;
        }

        public void DeleteDocument(string location, string fileName)
        {
            string combinedPath = Path.Combine(location, fileName);
            int retryCount = 0;
            int numberOfRetries = 10;

            do
            {
                try
                {
                    File.Delete( combinedPath );
                    return;
                }
                //Catch just in case the file is read only
                catch (UnauthorizedAccessException)
                {
                    if (retryCount == numberOfRetries)
                    {
                        Debug.WriteLine( $"Could not delete file after {numberOfRetries} attempts." );
                        throw;
                    }
                }
                //Catch just in case the file is being accessed by another process only
                catch (IOException e) when (CheckForErrorSharingViolation(e.HResult))
                {
                    if (retryCount == numberOfRetries)
                    {
                        Debug.WriteLine($"Could not delete file after {numberOfRetries} attempts.");
                        throw;
                    }
                }
                retryCount++;
                Thread.Sleep(100);
            } while (retryCount < numberOfRetries);
        }
        
        //TODO: Is there a better way to handle saving a document for a test? E.G. a randomly generated filename in the specified location?
        //TODO: Assess if part of this should be pulled out and put into the wrapper Utilities.cs class
        public void SaveDocument(string location, string filename)
        {
            //Check if doc with same name already exists in location before saving
            string formattedPath = Path.Combine(location, filename);
            if (File.Exists(formattedPath))
            {
                filename = GetUniqueFileName( formattedPath );
            }
            Mouse.Click();
            Mouse.Click(_NotepadWindow.Save);
            Mouse.Click(_SaveAsWindow.FindSaveLocationToolbar);
            _SaveAsWindow.SaveAsBox.EditableItem = filename;
            _SaveAsWindow.SaveButton.SetFocus();
            SendKeys.SendWait("{ENTER}");
        }

        //TODO: Is this needed anymore now that the tests delete files before running?
        public static string GetUniqueFileName(string filePath)
        {
            string uniqueFileName = filePath;
            for (int i = 0; File.Exists(uniqueFileName); uniqueFileName = AppendFileName(filePath, $" ({++i})"))
            { }
            return uniqueFileName;
        }

        public static string AppendFileName(string filePath, string toAppend)
        {
            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);
            return Path.Combine(dir ?? "", string.Concat(fileName, toAppend, extension));
        }

        private static bool CheckForErrorSharingViolation(int hResult)
        {
            return (uint)hResult == 0x80070020;
        }
    }
}