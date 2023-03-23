using IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies;
using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class SingleDependencyTests
    {
        // Test for...
        // Adding a dependency that itself does not have a satisfied dependency
        // A test block output is successfully used in a subsequent test block
        // Do we need to test asking for a null type?
        // Do we need to test returning a null type?

        [Fact]
        public void ExecuteTestWithAvailableInstanceForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteTestWithAvailableInstanceForTestBlockProperty()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithProperty>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteTestWithAvailableInstanceForTestBlockConstructor()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteTestBlockWitNonSettablePropertyDoesNotThrow()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithPropertyWithNoSetter>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        // Note: the following tests should be exercising out of the box MS DI functionality.
        // The purpose is just to ensure that these methods don't ever accidentally get decoupled from the underlying MS service provider.
        // That is also why we aren't extensively testing the same scenarios as above for the AddDependencyService method
        [Fact]
        public void ExecuteTestWithAvailableServiceForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyService<ExampleImplementation>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForOwnType>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteTestWithAvailableGenericArgumentAndInstanceForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance<IExampleDataInterface>(new ExampleImplementation())
                .AddTestBlock<ExampleTestBlockWithExecuteArgForInterface>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteTestWithAvailableGenericArgumentsForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyService<IExampleDataInterface, ExampleImplementation>()
                .AddTestBlock<ExampleTestBlockWithExecuteArgForInterface>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteTestWithAvailableFactoryForExecuteArg()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyService<ExampleImplementation>(new ExampleFactory().DoesNotThrow)
                .AddTestBlock<ExampleTestBlockForFactoryWithExecuteArg>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }
    }
}
