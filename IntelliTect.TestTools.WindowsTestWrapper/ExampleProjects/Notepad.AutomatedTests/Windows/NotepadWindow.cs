using System;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;

namespace Notepad.AutomatedTests.Windows
{
    public class NotepadWindow : DesktopControls
    {
        //Set the AutWindowTitle for use in searching for controls.
        //This will be different if the application spawns a new window that is not a child control of the application under test
        //If a new parent window is created by the application under test, create a new class derived from DesktopControls where
        //AutWindowTitle is set to that window's title.
        public override string AutWindowTitle => Aut?.Title;
        //Control type for the window you want to manipulate. Obtain through Inspect.exe, Spy++, or the CodedUI test builder inspect
        public override string WindowClassName => Aut?.ClassName;

        //public static void LaunchApplicationUnderTest()
        //{
        //    LaunchApplication( Environment.SystemDirectory + "\\notepad.exe" );
        //}

        //In the control hierarchy, the only edit control available is the one we want to manipulate
        public WinEdit NotepadEdit
        {
            get { return FindControlByChild( c => new WinEdit( c ) ); }
        }

        public WinMenuItem File
        {
            get { return FindControlByName( "File", c => new WinMenuItem( c ) ); }
        }

        public WinMenuItem Save
        {
            get { return FindControlByName( "Save	Ctrl+S", c => new WinMenuItem( c ), File ); }
        }
    }
}