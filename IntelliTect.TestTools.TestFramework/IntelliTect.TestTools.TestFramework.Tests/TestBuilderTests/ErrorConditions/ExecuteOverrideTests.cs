using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests.ErrorConditions
{
    public class ExecuteOverrideTests : TestBase
    {
        [Fact]
        public void BuildWithMismatchedOverrideAndNoOtherMatchingThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>(1);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result, 
                2,
                ErrorMessages.MissingInputError,
                ErrorMessages.MismatchedExecuteOverrideError);
        }

        [Fact]
        public void BuildWithTooManyOverridesAndNoOtherMatchingThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing1", true);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                2,
                ErrorMessages.MissingInputError,
                ErrorMessages.TooManyExecuteOverridesError);
        }

        [Fact]
        public void BuildWithMismatchedOverrideAndOneMatchingThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>(1);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result, 
                1, 
                ErrorMessages.MismatchedExecuteOverrideError);
        }

        [Fact]
        public void BuildWithTooManyOverridesAndOneMatchingThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing1", true);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                1,
                ErrorMessages.TooManyExecuteOverridesError);
        }

        [Fact]
        public void BuildWithDuplicateExecuteOverridesThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing1", "Testing2");

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                1,
                ErrorMessages.AlreadyAddedError);
        }

        [Fact]
        public void BuildWithExecuteOverrideOutOfOrderReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>(true)
                .AddTestBlock<ExampleTestBlockWithStringReturn>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                2,
                ErrorMessages.MismatchedExecuteOverrideError,
                ErrorMessages.MissingInputError);
        }
    }
}
