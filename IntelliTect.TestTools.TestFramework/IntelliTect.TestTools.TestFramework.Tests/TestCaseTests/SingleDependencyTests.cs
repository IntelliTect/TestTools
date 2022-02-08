using IntelliTect.TestTools.TestFramework.Tests.TestData;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class SingleDependencyTests
    {
        // Test for...
        // Adding a dependency that itself does not have a satisfied dependency
        // A test block output is successfully used in a subsequent test block

        [Fact]
        public void ExecuteTestWithAvailableInstanceForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void ExecuteTestWithAvailableInstanceForTestBlockProperty()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithProperty>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void ExecuteTestWithAvailableInstanceForTestBlockConstructor()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void ExecuteTestBlockWitNonSettablePropertyDoesNotThrow()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithPropertyWithNoSetter>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        // Note: the following tests should be exercising out of the box MS DI functionality.
        // The purpose is just to ensure that these methods don't ever accidentally get decoupled from the underlying MS service provider.
        // That is also why we aren't extensively testing the same scenarios as above for the AddDependencyService method
        [Fact]
        public void ExecuteTestWithAvailableServiceForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyService<ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void ExecuteTestWithAvailableGenericArgumentAndInstanceForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance<IExampleDataInterface>(new ExampleInterface())
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void ExecuteTestWithAvailableGenericArgumentsForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyService<IExampleDataInterface, ExampleInterface>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .Build();

            // Act / Assert
            tc.Execute();
        }

        [Fact]
        public void ExecuteTestWithAvailableFactoryForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyService<ExampleInterface>(new ExampleDataThingFactory().ExampleDataThing)
                .AddTestBlock<ExampleTestBlockForFactoryWithExecuteArg>()
                .Build();

            // Act / Assert
            tc.Execute();
        }


        // OLD TESTS
        // REFACTOR AND MOVE AS NEEDED


        //[Fact]
        //public void FetchByObjectInstanceForMultipleDependencies()
        //{
        //    TestBuilder builder = new();
        //    builder
        //        .AddDependencyInstance("Testing")
        //        .AddDependencyInstance(1234)
        //        .AddTestBlock<ExampleTestBlockWithMultipleDependencies>()
        //        .ExecuteTestCase()
        //        ;
        //}

        //[Fact]
        //public void AddFinallyBlockThrowsExpectedException()
        //{
        //    TestBuilder builder = new();
        //    builder
        //        .AddTestBlock<ExampleTestBlockWithReturn>(false)
        //        .AddFinallyBlock<ExampleFinallyBlock>()
        //        .ExecuteTestCase();
        //}

        //// Actually... this probably shouldn't throw since it's a "finally" block meant to clean stuff up
        //// Figure out the right behavior and fix test before moving further with finally blocks
        //[Fact]
        //public void AddFinallyBlockDoesNotThrowIfExceptionOccursInFinally()
        //{
        //    TestBuilder builder = new();
        //    builder
        //        .AddTestBlock<ExampleTestBlockWithReturn>(true)
        //        .AddFinallyBlock<ExampleFinallyBlock>()
        //        .ExecuteTestCase();
        //}

        //// How do we verify this is working correctly?
        //[Fact]
        //public void AddFinallyBlockExecutesAfterException()
        //{
        //    TestBuilder builder = new();
        //    builder
        //        .AddDependencyInstance(true)
        //        .AddTestBlock<ExampleTestBlockWithMultipleExecuteMethods>()
        //        .AddFinallyBlock<ExampleFinallyBlock>();

        //    Assert.Throws<TestCaseException>(() => builder.ExecuteTestCase());
        //}
    }
}
