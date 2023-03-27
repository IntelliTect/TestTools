using IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies;
using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class FinallyExecutionTests
    {

        [Fact]
        public void FinallyBlockThrowsExpectedExceptionWhenNotOverridingDefaultFinallyBehavior()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddFinallyBlock<ExampleFinallyBlock>()
                .Build();

            // Act
            var ex = Assert.Throws<AggregateException>(() => tc.Execute());

            // Assert
            Assert.NotNull(ex.InnerExceptions);
            Assert.Single(ex.InnerExceptions);
            Assert.Contains("Test case succeeded", 
                ex.Message, 
                StringComparison.InvariantCultureIgnoreCase);
            Assert.True(tc.Passed, "Test case did not get marked as Passed when we expected it.");
        }

        [Fact]
        public void TestBlockAndFinallyBlockThrowsExpectedExceptionWhenNotOverridingDefaultFinallyBehavior()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance(false)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddFinallyBlock<ExampleFinallyBlock>()
                .Build();

            // Act
            var ex = Assert.Throws<AggregateException>(() => tc.Execute());

            // Assert
            Assert.NotNull(ex.InnerExceptions);
            Assert.Equal(2, ex.InnerExceptions.Count);
            Assert.Contains("Test case failed and finally blocks failed",
                ex.Message,
                StringComparison.InvariantCultureIgnoreCase);
            Assert.False(tc.Passed, "Test case did not get marked as Failed when we expected it.");
        }

        [Fact]
        public void FinallyBlockDoesNotThrowExceptionWhenOverridingDefaultFinallyBehavior()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddFinallyBlock<ExampleFinallyBlock>()
                .Build();
            tc.ThrowOnFinallyBlockException = false;

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed, "Test case did not get marked as Passed when we expected it.");
        }

        [Fact]
        public void OnlyTestBlockThrowsExpectedExceptionWhenOverridingDefaultFinallyBehavior()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance(false)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddFinallyBlock<ExampleFinallyBlock>()
                .Build();
            tc.ThrowOnFinallyBlockException = false;

            // Act
            Assert.Throws<TestCaseException>(() => tc.Execute());

            // Assert
            Assert.False(tc.Passed, "Test case did not get marked as Failed when we expected it.");
        }
    }
}
