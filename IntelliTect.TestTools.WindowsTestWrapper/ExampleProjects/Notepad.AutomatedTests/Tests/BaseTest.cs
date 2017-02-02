using System;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Notepad.AutomatedTests.Helpers;
using Notepad.AutomatedTests.Windows;

//TODO: Look into making this an inherited class
namespace Notepad.AutomatedTests.Tests
{
    /// <summary>
    /// Summary description for BaseTest
    /// </summary>
    [CodedUITest]
    public partial class BaseTest : BaseTestInherit
    {
        public override string ApplicationLocation => Environment.SystemDirectory + "\\notepad.exe";
        protected readonly NotepadWindow NotepadWindow = new NotepadWindow();
        protected readonly SaveAsWindow SaveAsWindow = new SaveAsWindow();
        protected readonly NotepadWindowHelpers NotePadWindowHelpers = new NotepadWindowHelpers();
    }
}