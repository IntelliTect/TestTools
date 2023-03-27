using IntelliTect.TestTools.TestFramework.Tests.TestData.Dependencies;
using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class MultipleDependencyTests
    {
        [Fact]
        public void ReturnDuplicateTypesDoesNotThrow()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddTestBlock<ExampleTestBlockWithBoolReturn>(true)
                .AddTestBlock<ExampleTestBlockWithBoolReturn>(true)
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void FetchByObjectInstanceForMultipleDependencies()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Testing")
                .AddDependencyInstance(1234)
                .AddTestBlock<ExampleTestBlockWithMultipleDependencies>()
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }
    }
}
