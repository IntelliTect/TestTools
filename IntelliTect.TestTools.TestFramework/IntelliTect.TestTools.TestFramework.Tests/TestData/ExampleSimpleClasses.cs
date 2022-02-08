using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestData
{
    public class ExampleTestBlockWithExecuteArg : TestBlock
    {
        public void Execute(string input)
        {
            Assert.Equal("Testing", input);
        }
    }

    public class ExampleTestBlockWithProperty : TestBlock
    {
        public string? Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input);
        }
    }

    public class ExampleTestBlockWithConstructor : TestBlock
    {
        public ExampleTestBlockWithConstructor(string input)
        {
            Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", Input);
        }

        private string Input { get; }
    }

    public class ExampleTestBlockWithMultipleDependencies : TestBlock
    {
        public string? InputText { get; set; }

        public void Execute(int inputNumber)
        {
            Assert.Equal("Testing", InputText);
            Assert.Equal(1234, inputNumber);
        }
    }

    public class ExampleTestBlockWithMultipleExecuteArgs : TestBlock
    {
        public void Execute(int inputNumber1, int inputNumber2)
        {
            Assert.Equal(inputNumber1, inputNumber2);
        }
    }

    public class ExampleTestBlockWithStringReturn : TestBlock
    {
        public string Execute()
        {
            return "Testing";
        }
    }

    public class ExampleTestBlockWithBoolReturn : TestBlock
    {
        public bool Execute(bool arg)
        {
            if (!arg)
            {
                throw new DivideByZeroException("Test failure");
            }
            else
            {
                return !arg;
            }
        }
    }
}
