using IntelliTect.TestTools.TestFramework.Tests.TestData;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class Perf
    {
        private readonly int _Iterations = 5000000;

        [Fact]
        public void TestWithProvider()
        {
            for (int i = 0; i < _Iterations; i++)
            {
                TestBuilder builder = new();
                builder.AddDependencyInstance("Testing")
                    .AddTestBlock<ExampleTestBlockWithConstructor>();

                builder.BuildWithProvider();
            }

        }

        [Fact]
        public void TestWithoutProvider()
        {
            for (int i = 0; i < _Iterations; i++)
            {
                TestBuilder builder = new();
                builder.AddDependencyInstance("Testing")
                    .AddTestBlock<ExampleTestBlockWithConstructor>();

                builder.BuildWithoutProvider();
            }
        }
    }
}
