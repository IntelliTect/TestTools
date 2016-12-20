using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.AutomatedTests.Tests
{
    [CodedUITest]
    public class ExampleCalculatorTest : BaseTest
    {
        //TODO: Change these to data drive tests
        [TestMethod]
        public void SimpleAddition()
        {
            Mouse.Click(CalcWindow.CalculatorButton( "1" ));
            Mouse.Click(CalcWindow.CalculatorButton(Add));
            Mouse.Click(CalcWindow.CalculatorButton("1"));
            Mouse.Click(CalcWindow.CalculatorButton(EqualButton));
            Assert.IsTrue(CalcHelpers.VerifyResults( "2" ), "Calculator showed incorrect result");
            Assert.IsFalse( CalcHelpers.VerifyResults( "0" ), "Result never changed from default 0");
        }

        [TestMethod]
        public void SimpleSubtraction()
        {
            Mouse.Click(CalcWindow.CalculatorButton("2"));
            Mouse.Click(CalcWindow.CalculatorButton(Subtract));
            Mouse.Click(CalcWindow.CalculatorButton("1"));
            Mouse.Click(CalcWindow.CalculatorButton(EqualButton));
            Assert.IsTrue(CalcHelpers.VerifyResults("1"), "Calculator showed incorrect result");
            Assert.IsFalse(CalcHelpers.VerifyResults("0"), "Result never changed from default 0");
        }

        [TestMethod]
        public void DecimalCheck()
        {
            Mouse.Click(CalcWindow.CalculatorButton("1"));
            Mouse.Click(CalcWindow.CalculatorButton(Decimal));
            Mouse.Click(CalcWindow.CalculatorButton("2"));
            Mouse.Click(CalcWindow.CalculatorButton("5"));
            Mouse.Click(CalcWindow.CalculatorButton(Add));
            Mouse.Click(CalcWindow.CalculatorButton("1"));
            Mouse.Click(CalcWindow.CalculatorButton(Decimal));
            Mouse.Click(CalcWindow.CalculatorButton("5"));
            Mouse.Click(CalcWindow.CalculatorButton(EqualButton));
            Assert.IsTrue(CalcHelpers.VerifyResults("2.75"), "Calculator showed incorrect result");
            Assert.IsFalse(CalcHelpers.VerifyResults("0"), "Result never changed from default 0");
        }
    }
}
