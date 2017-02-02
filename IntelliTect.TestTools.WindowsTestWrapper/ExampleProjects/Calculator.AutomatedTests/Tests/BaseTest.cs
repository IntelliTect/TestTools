using System;
using Calculator.AutomatedTests.Helpers;
using Calculator.AutomatedTests.Windows;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;

namespace Calculator.AutomatedTests.Tests
{
    [CodedUITest]
    public partial class BaseTest : BaseTestInherit
    {
        public override string ApplicationLocation => Environment.SystemDirectory + @"\calc.exe";
        protected readonly CalculatorWindow CalcWindow = new CalculatorWindow();
        protected readonly CalculatorHelpers CalcHelpers = new CalculatorHelpers();
    }
}
