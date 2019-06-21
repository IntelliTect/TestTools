using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBuilderTests
    {
        [Fact]
        public void FetchByObjectInstanceForExecuteArg()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockProperty()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockConstructor()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>()
                .ExecuteTestCase();
        }
    }

    public class ExampleTestBlockWithExecuteArg : ITestBlock
    {
        public void Execute(string input)
        {
            Assert.Equal("Testing", input);
        }
    }

    public class ExampleTestBlockWithProperty : ITestBlock
    {
        public string Input { get; set; }

        public void Execute()
        {
            Assert.Equal("Testing", Input);
        }
    }

    public class ExampleTestBlockWithConstructor : ITestBlock
    {
        public ExampleTestBlockWithConstructor(string input)
        {
            _Input = input;
        }

        public void Execute()
        {
            Assert.Equal("Testing", _Input);
        }

        private string _Input { get; }
    }
}
