using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests.ErrorConditions
{
    public class ExecuteMethodErrors
    {
        [Fact]
        public void BuildWithMissingExecuteMethodThrowsInvalidOperationException()
        {
            // Arrange
            TestBuilder builder = new();

            // Act
            var result = Assert.Throws<InvalidOperationException>(() =>
                builder.AddTestBlock<TestBlock>());

            // Assert
            Assert.Contains(
                ErrorMessages.ExecuteError,
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void BuildWithTwoExecuteMethodsThrowsInvalidOperationException()
        {
            // Arrange
            TestBuilder builder = new();

            // Act
            var result = Assert.Throws<InvalidOperationException>(() =>
                builder.AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>());

            // Assert
            Assert.Contains(
                ErrorMessages.ExecuteError,
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class ExampleTestBlockWithMultipleExecuteMethods : TestBlock
    {
        public void Execute()
        {
        }

        public void Execute(string input)
        {
            Assert.Equal("Testing", input);
        }
    }
}
