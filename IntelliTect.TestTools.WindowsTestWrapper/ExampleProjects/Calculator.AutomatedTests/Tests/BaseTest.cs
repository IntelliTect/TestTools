using Calculator.AutomatedTests.Helpers;
using Calculator.AutomatedTests.Windows;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.AutomatedTests.Tests
{
    [CodedUITest]
    public partial class BaseTest
    {
        protected readonly CalculatorWindow CalcWindow = new CalculatorWindow();
        protected readonly CalculatorHelpers CalcHelpers = new CalculatorHelpers();

        [TestInitialize]
        public void MyTestInitialize()
        {
            GenericPlaybackSettings.SetPlaybackSettings();
            CalcWindow.LaunchApplicationUnderTest();
        }

        //[TestCleanup]
        //public void MyTestCleanup()
        //{
            
        //}
    }
}
