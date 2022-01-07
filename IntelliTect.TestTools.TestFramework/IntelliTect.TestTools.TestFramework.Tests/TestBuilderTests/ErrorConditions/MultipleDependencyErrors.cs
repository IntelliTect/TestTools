using IntelliTect.TestTools.TestFramework.Tests.TestData;
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
            ValidateAggregateException(result, 2);
        }

        [Fact]
        public void BuildWithMismatchedDependencyThrowsAggregateException()
        {
            // Arrange
            TestBuilder builder = new TestBuilder()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddDependencyInstance(true);

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(result, 2);
        }

        //[Fact]
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
                .AddTestBlock<ExampleTestBlockWithBoolReturn>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>();
            //builder = AddCommonBlock(builder)
            //    ;

            // Act
            var result = Assert.Throws<AggregateException>(() =>
                builder.Build());

            // Assert
            ValidateAggregateException(result, 2);
        }
    }
}
