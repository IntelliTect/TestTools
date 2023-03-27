using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestCaseTests
{
    public class ExecuteArgumentOverrides
    {
        // Test for...
        // One execute override successfully overrides something from DI container
        // Two different execute overrides successfully inject

        [Fact]
        public void ExecuteOverrideInjectsCorrectDependency()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("This will fail.")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>("Testing")
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteOverrideWithMultipleDependenciesInjectsCorrectDependency()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("This will fail.")
                .AddDependencyInstance(1234)
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteArgs>("Testing")
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteMultipleOverridesWithMultipleDependenciesInjectsCorrectDependency()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("This will fail.")
                .AddDependencyInstance(4321)
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteArgs>("Testing", 1234)
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }

        [Fact]
        public void ExecuteMultipleOverridesOutOfOrderInjectsCorrectDependency()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddTestBlock<ExampleTestBlockWithMultipleExecuteArgs>(1234, "Testing")
                .Build();

            // Act
            tc.Execute();

            // Assert
            Assert.True(tc.Passed);
        }
    }
}
