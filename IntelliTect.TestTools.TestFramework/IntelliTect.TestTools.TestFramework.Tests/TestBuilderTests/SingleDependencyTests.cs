using IntelliTect.TestTools.TestFramework.Tests.TestData;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests
{
    public class SingleDependencyTests
    {
        [Fact]
        public void Testing1()
        {
            TestBuilder builder = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            builder.Build();
        }

        [Fact]
        public void Testing2()
        {
            TestBuilder builder = new TestBuilder()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing");

            TestCase tc = builder.Build();
            tc.Execute();
        }

        // Check for logger
        // Check for removed logger

        // Check for singleton
        // Check for scoped
        // Check for output
        // Find for constructor
        // Find for property
        // Find for execute param

        // TO DO:
        // Check for test block argument
    }
}
