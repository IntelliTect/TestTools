using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using Xunit;
using Xunit.Sdk;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBuilderTests
    {
        [Fact]
        public void BuildWithMissingExecuteMethodThrowsInvalidOperationException()
        {
            TestBuilder builder = new();
            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() =>
                builder
                    .AddTestBlock<TestBlock>()
                    .Build());
            Assert.Contains(
                "There must be one and only one Execute method",
                result.Message, 
                StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void BuildWithTwoExecuteMethodsThrowsInvalidOperationException()
        {
            //ExampleTestBlockWithMultipleExecuteMethods
            TestBuilder builder = new();
            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() =>
                builder
                    .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>()
                    .Build());
            Assert.Contains(
                "There must be one and only one Execute method",
                result.Message, 
                StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void BuildWithMissingDependencyInstanceThrowsInvalidOperationException()
        {
            TestBuilder builder = new();
            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => 
                builder
                    .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                    .Build());
            Assert.Contains(
                "unable to satisfy input",
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void BuildWithMismatchedDependencyInstanceThrowsInvalidOperationException()
        {
            TestBuilder builder = new();
            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() =>
                builder
                    .AddDependencyInstance(1)
                    .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                    .Build());
            Assert.Contains(
                "unable to satisfy input",
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void BuildWithMismatchedExecuteReturnThrowsInvalidOperationException()
        {
            TestBuilder builder = new();
            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() =>
                builder
                    .AddTestBlock<ExampleTestBlockWithReturn>(true)
                    .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                    .Build());
            Assert.Contains(
                "unable to satisfy input",
                result.Message,
                StringComparison.InvariantCultureIgnoreCase);
        }

        //[Fact]
        //public void BuildWithMissingDependencyInstanceThrowsInvalidOperationException()
        //{
        //    TestBuilder builder = new();
        //    InvalidOperationException result = Assert.Throws<InvalidOperationException>(() =>
        //        builder
        //            .AddTestBlock<ExampleTestBlockWithExecuteArg>()
        //            .Build());
        //    Assert.Contains(
        //        "unable to satisfy input",
        //        result.Message,
        //        StringComparison.InvariantCultureIgnoreCase);
        //}








        // OLD TESTS
        // REFACTOR AND MOVE AS NEEDED
        [Fact]
        //[TestCase]
        public void FetchByObjectInstanceForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .Build()
                //.ExecuteTestCase()
                ;
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockProperty()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForTestBlockConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByObjectInstanceForMultipleDependencies()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddDependencyInstance(1234)
                .AddTestBlock<ExampleTestBlockWithMultipleDependencies>()
                .ExecuteTestCase();
        }

        [Fact]
        public void PassExecuteArgumentsViaAddTestBlockParams()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void MismatchedCountAddTestBlockParamsAndExecuteArgsFails()
        {
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing", "Testing2");

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void MismatchedTypeAddTestBlockParamsAndExecuteArgsFails()
        {
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithExecuteArg>(1234);

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void FetchByServiceForConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithConstructorForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByServiceForProperty()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithPropertyForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByServiceForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByImplementationForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance<IExampleDataInterface>(new ExampleInterface())
                .AddTestBlock<ExampleTestBlockWithExecuteArgForInterface>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByImplementationAndTypeForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<IExampleDataInterface, ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForInterface>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void FetchByFactoryForConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithConstructor>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByFactoryForProperty()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithProperty>()
                .ExecuteTestCase();
        }

        [Fact]
        public void FetchByFactoryForExecuteArg()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithExecuteArg>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void AddTwoServicesOfSameTypeToServiceAndFetch()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyService<ExampleInterface>()
                .AddDependencyService<ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        // This test probably isn't necessary. This is MS DI out-of-the-box functionality
        [Fact]
        public void AddTwoInstancesOfSameTypeToServiceAndFetch()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(new ExampleInterface { Testing = "Testing2" })
                .AddDependencyInstance(new ExampleInterface { Testing = "Testing" })
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .ExecuteTestCase();
        }

        [Fact]
        public void TestBlockWithPropertyWithNoSetterDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithPropertyWithNoSetter>()
                .ExecuteTestCase();
        }

        [Fact]
        public void TestBlockWithMultipleExecuteMethodsThrows()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>();

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void TestBlockThatFailsThrowsCorrectException()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Bad Value");

            try
            {
                builder.ExecuteTestCase();
            }
            catch (TestCaseException ex)
            {
                Assert.Equal(typeof(EqualException), ex.InnerException.GetType());
            }
        }

        [Fact]
        public void AddLoggerReturnsCorrectLogger()
        {
            TestBuilder builder = new();
            builder
                .AddLogger<ExampleLogger>()
                .AddTestBlock<ExampleLoggerUsage>();

            Assert.Throws<NotImplementedException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void AddFinallyBlockThrowsExpectedException()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithReturn>(false)
                .AddFinallyBlock<ExampleFinallyBlock>()
                .ExecuteTestCase();
        }

        // Actually... this probably shouldn't throw since it's a "finally" block meant to clean stuff up
        // Figure out the right behavior and fix test before moving further with finally blocks
        [Fact]
        public void AddFinallyBlockDoesNotThrowIfExceptionOccursInFinally()
        {
            TestBuilder builder = new();
            builder
                .AddTestBlock<ExampleTestBlockWithReturn>(true)
                .AddFinallyBlock<ExampleFinallyBlock>()
                .ExecuteTestCase();
        }

        // How do we verify this is working correctly?
        [Fact]
        public void AddFinallyBlockExecutesAfterException()
        {
            TestBuilder builder = new();
            builder
                .AddDependencyInstance(true)
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>()
                .AddFinallyBlock<ExampleFinallyBlock>();

            Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        }

        [Fact]
        public void OverridingLoggerDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void RemovingLoggerTwiceDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .RemoveLogger()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void AddingLoggerThanRemovingDoesNotThrow()
        {
            TestBuilder builder = new();
            builder
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void OverrideTestCaseNameWithConstructor()
        {
            TestBuilder builder = new();
            builder
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void OverrideTestCaseNameWithMethod()
        {
            TestBuilder builder = new();
            builder
                .AddTestCaseId(123456)
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void OverrideTestCaseNameWithMethodOverride()
        {
            TestBuilder builder = new();
            builder
                //.OverrideTestCaseKey("Testing")
                .AddLogger<ExampleLogger>()
                .RemoveLogger()
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .ExecuteTestCase();
        }

        [Fact]
        public void PropertyWithNoMatchingTypeInDiThrowsInvalidOperation()
        {
            TestBuilder builder = new();
            builder.AddTestBlock<ExampleTestBlockWithProperty>();

            Exception ex = Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
            Assert.Equal(typeof(InvalidOperationException), ex.InnerException.GetType());
        }
    }
}
