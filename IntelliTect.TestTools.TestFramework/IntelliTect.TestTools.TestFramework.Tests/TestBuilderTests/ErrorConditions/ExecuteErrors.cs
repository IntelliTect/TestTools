using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests.ErrorConditions
{
    public class ExecuteErrors
    {
        [Fact]
        public void BuildWithMissingExecuteMethodThrowsInvalidOperationException()
        {
            // Arrange
            TestBuilder builder = new();
            builder.AddTestBlock<TestBlock>();

            // Act
            var result = Assert.Throws<InvalidOperationException>(() =>
                builder.Build());

            // Assert
            Assert.Contains(
                ErrorMesssages.ExecuteError,
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void BuildWithTwoExecuteMethodsThrowsInvalidOperationException()
        {
            // Arrange
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>();

            // Act
            var result = Assert.Throws<InvalidOperationException>(() =>
                builder.Build());

            // Assert
            Assert.Contains(
                ErrorMesssages.ExecuteError,
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class ExampleTestBlockWithMultipleExecuteMethods : TestBlock
    {
        public void Execute()
        {
            Assert.True(true);
        }

        public void Execute(string input)
        {
            Assert.Equal("Tetsing", input);
        }
    }
}
