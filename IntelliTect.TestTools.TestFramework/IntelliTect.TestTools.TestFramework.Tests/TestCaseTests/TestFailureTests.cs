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
                .AddTestBlock<TestData.ExampleTestBlockWithBoolReturn>()
                .AddTestBlock<TestData.ExampleTestBlockWithBoolReturn>()
                .Build();

            var ex = Assert.Throws<TestCaseException>(() => tc.Execute());
            Assert.NotNull(ex.InnerException);
            Assert.Equal(typeof(DivideByZeroException), ex.InnerException!.GetType());
            Assert.Equal("test failure", ex.InnerException.Message, ignoreCase: true);
        }

        // Finally blocks still execute
        // Dependencies of dependencies fail
    }
}
