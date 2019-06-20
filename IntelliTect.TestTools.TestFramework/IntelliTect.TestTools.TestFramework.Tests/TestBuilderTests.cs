using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBuilderTests
    {
        [Fact]
        public void AddServiceToContainer()
        {
            TestBuilder builder = new TestBuilder();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlock>();
        }
    }

    public class ExampleTestBlock : ITestBlock
    {
        public void Execute(string input)
        {
            Assert.Equal("Testing", input);
        }
    }
}
