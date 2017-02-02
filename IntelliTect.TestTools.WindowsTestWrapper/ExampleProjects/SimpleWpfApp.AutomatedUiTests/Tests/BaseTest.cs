using System.IO;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWpfApp.AutomatedUiTests.Windows;

//TODO: Look into making this an inherited class
namespace SimpleWpfApp.AutomatedUiTests.Tests
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class BaseTest : BaseTestInherit
    {
        public override string ApplicationLocation => Path.GetFullPath(@"..\..\..\ExampleProjects\SimpleWpfApp\bin\Debug\SimpleWpfApp.exe");
        protected readonly SimpleWpfAppWindow SimpleAppWindow = new SimpleWpfAppWindow();
    }
}
