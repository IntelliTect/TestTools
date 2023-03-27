using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests.ErrorConditions
{
    public class MultipleDependencyErrors : TestBase
    {
        [Fact]
        public void BuildWithMissingDependencyThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new TestBuilder()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result, 
                2,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithMismatchedDependencyThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new TestBuilder()
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                2,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithMismatchedDependencyAsTestBlockParamThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>(true)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>(true);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                4,
                ErrorMessages.MissingInputError,
                ErrorMessages.MismatchedExecuteOverrideError);
        }

        [Fact]
        public void BuildWithMismatchedTestBlockReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                2,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithExecuteOverrideAndMismatchedTestBlockReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithBoolReturn>(true)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                2,
                ErrorMessages.MissingInputError);
        }
    }
}
