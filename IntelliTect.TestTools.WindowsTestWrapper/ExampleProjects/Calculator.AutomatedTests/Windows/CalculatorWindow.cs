using System;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;

namespace Calculator.AutomatedTests.Windows
{
    public class CalculatorWindow : DesktopControls
    {
        public override string AutWindowTitle => Aut?.Title;
        public override string WindowClassName => Aut?.ClassName;

        //public void LaunchApplicationUnderTest()
        //{
        //    LaunchApplication(Environment.SystemDirectory + @"\calc.exe");
        //}

        public WinButton CalculatorButton( string buttonName )
        {
            return FindControlByName( buttonName, c => new WinButton( c ) );
        }

        public WinWindow FindResultsControl()
        {
            return FindControlByName( "Result", c => new WinWindow(c) );
        }
    }
}
