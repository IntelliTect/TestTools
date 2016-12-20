using Calculator.AutomatedTests.Windows;
using Microsoft.VisualStudio.TestTools.UITesting;

namespace Calculator.AutomatedTests.Helpers
{
    public class CalculatorHelpers
    {
        readonly CalculatorWindow _CalculatorWindow = new CalculatorWindow();
        public bool VerifyResults(string expectedValue)
        {
            return expectedValue == _CalculatorWindow.FindResultsControl().Name;
        }
    }
}