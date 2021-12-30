using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests
{
    public class MultipleDependencyErrors : TestBase
    {
        [Fact]
        public void BuildWithMissingDependencyThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = StartTestCaseWithMissingDependency()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(result, 2);
        }

        [Fact]
        public void BuildWithMismatchedDependencyThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = StartTestCaseWithMissingDependency()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddDependencyInstance(true);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(result, 2);
        }

        //[Fact(Skip = "Functionality in progress.")]
        //public void BuildWithMismatchedDependencyAsTestBlockParamThrowsAggregateException()
        //{
        //    // Arrange
        //    TestBuilder builder = new();
        //    builder
        //        .AddTestBlock<ExampleTestBlockWithExecuteArg>(true)
        //        .AddTestBlock<ExampleTestBlockWithExecuteArg>(true);

        //    // Act
        //    var result = Assert.Throws<AggregateException>(() =>
        //        builder.Build());

        //    // Assert
        //    ValidateAggregateException(result, 2);
        //}

        [Fact]
        public void BuildWithMismatchedTestBlockReturnThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithReturn>();
            builder = StartTestCaseWithMissingDependency(builder)
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(result, 2);
        }
    }
}
