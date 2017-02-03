using System.Diagnostics;
using System.IO;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        //TODO: Problem with inheriting in this manner, these override ALL default values.
        //TODO: Is there a better way to handle launching the application instead of in TestInitialize?
        //[TestInitialize]
        //public new void MyTestInitialize()
        //{
        //    Debug.Write("Example of custom test initiliaze");
        //}

        //[TestCleanup]
        //public void MyTestCleanup()
        //{
        //    //Debug.Write( "Example of custom test cleanup" );
        //}
    }
}
