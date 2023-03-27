using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks
{
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
        public void Execute(string inputText, int inputNumber)
        {
            Assert.Equal("Testing", inputText);
            Assert.Equal(1234, inputNumber);
        }
    }
}
