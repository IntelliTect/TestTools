using System;
using System.IO;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Notepad.AutomatedTests.Helpers;
using Notepad.AutomatedTests.Windows;

namespace Notepad.AutomatedTests.Tests
{
    /// <summary>
    /// Summary description for BaseTest
    /// </summary>
    [CodedUITest]
    public partial class BaseTest : BaseTestInherit
    {
        public override string ApplicationLocation => Path.Combine( Environment.SystemDirectory, "notepad.exe" );
        protected readonly NotepadWindow NotepadWindow = new NotepadWindow();
        protected readonly SaveAsWindow SaveAsWindow = new SaveAsWindow();
        protected readonly NotepadWindowHelpers NotePadWindowHelpers = new NotepadWindowHelpers();
    }
}