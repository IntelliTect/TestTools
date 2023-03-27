using IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies;
using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests.ErrorConditions
{
    public class SingleDependencyErrors : TestBase
    {
        // Make sure to check for out of order returns/dependencies

        [Fact]
        public void BuildWithMissingDependencyInstanceThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result, 
                1,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithMismatchedDependencyInstanceThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new TestBuilder()
                .AddDependencyInstance(1)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                1,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithMismatchedExecuteReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                1,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithOutOfOrderReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithStringReturn>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                1,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void BuildWithExecuteOverrideMismatchedExecuteReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithBoolReturn>(true)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(
                result,
                1,
                ErrorMessages.MissingInputError);
        }

        [Fact]
        public void AddNullInstanceAndTypeThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TestBuilder()
                .AddDependencyInstance<IExampleDataInterface>(null!));
        }

        [Fact]
        public void AddNullInstanceThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new TestBuilder()
                .AddDependencyInstance(null!));
        }
    }
}
