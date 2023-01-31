using IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies;
using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class TestFailureTests
    {
        [Fact]
        public void TestFailureThrowsImmediatelyWithOriginalException()
        {
            TestBuilder builder = new();
            TestCase tc = builder
                .AddDependencyInstance(false)
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddTestBlock<ExampleTestBlockWithMultipleDependencies>(1)
                .Build();

            var ex = Assert.Throws<TestCaseException>(() => tc.Execute());
            Assert.False(tc.Passed);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<DivideByZeroException>(ex.InnerException);
            Assert.Equal("test failure", ex.InnerException!.Message, ignoreCase: true);
        }

        [Fact]
        public void DependencyWithMissingDependencyThrowsOriginalError()
        {
            TestCase tc = new TestBuilder()
                .AddDependencyService<AlwaysThrow>(new ExampleFactory().Throws)
                .AddDependencyService<SomeDependency>()
                .AddTestBlock<SomeTestBlock>()
                .Build();

            var ex = Assert.Throws<TestCaseException>(() => tc.Execute());
            Assert.False(tc.Passed);
            Assert.NotNull(ex.InnerException);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
            Assert.Contains("oops", ex.InnerException!.Message, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
