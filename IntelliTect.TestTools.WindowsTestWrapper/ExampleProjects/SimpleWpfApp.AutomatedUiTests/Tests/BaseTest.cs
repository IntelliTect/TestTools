using System;
using System.Diagnostics;
using System.IO;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWpfApp.AutomatedUiTests.Windows;

namespace SimpleWpfApp.AutomatedUiTests.Tests
{
    [CodedUITest]
    public class BaseTest : BaseTestInherit
    {
        public override string ApplicationLocation => Path.GetFullPath(@"..\..\..\IntelliTect.TestTools.WindowsTestWrapper\ExampleProjects\SimpleWpfApp\bin\Debug\SimpleWpfApp.exe");

        protected readonly SimpleWpfAppWindow SimpleAppWindow = new SimpleWpfAppWindow();
    }
}
