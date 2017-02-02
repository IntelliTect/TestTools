using System.IO;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using SimpleWpfApp.AutomatedUiTests.Windows;

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
