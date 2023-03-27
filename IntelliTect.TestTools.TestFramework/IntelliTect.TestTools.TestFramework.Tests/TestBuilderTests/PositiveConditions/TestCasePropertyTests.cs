using IntelliTect.TestTools.TestFramework.Tests.TestData.TestBlocks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests.TestBuilderTests
{
    public class TestCasePropertyTests
    {
        private const string _Name = "New Name";

        [Fact]
        public void TestCaseNameCanBeChanged()
        {
            // Arrange / Act
            TestCase tc = new TestBuilder()
                .AddTestCaseName(_Name)
                .Build();

            // Assert
            Assert.Equal(_Name, tc.TestCaseName);
        }

        [Fact]
        public void NullTestMethodNameIsOverriddenToGenericTestCaseName()
        {
            // Arrange / Act
            TestBuilder tb = new(null);
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal("UndefinedTestMethodName", tc.TestCaseName);
        }

        [Fact]
        public void TestCaseNameIsOverriddenToCallingMethod()
        {
            // Arrange / Act
            TestBuilder tb = new();
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal(nameof(TestCaseNameIsOverriddenToCallingMethod), tc.TestCaseName);
        }

        [Fact]
        public void TestCaseNameIsOverriddenByConstructorArg()
        {
            // Arrange / Act
            TestBuilder tb = new(_Name);
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal(_Name, tc.TestCaseName);
        }

        [Fact]
        public void NullTestMethodNameIsOverriddenToGenericTestMethodName()
        {
            // Arrange / Act
            TestBuilder tb = new(null);
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal("UndefinedTestMethodName", tc.TestMethodName);
        }

        [Fact]
        public void TestMethodNameIsOverriddenToCallingMethod()
        {
            // Arrange / Act
            TestBuilder tb = new();
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal(nameof(TestMethodNameIsOverriddenToCallingMethod), tc.TestMethodName);
        }

        [Fact]
        public void TestMethodNameIsOverriddenByConstructorArg()
        {
            // Arrange / Act
            TestBuilder tb = new(_Name);
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal(_Name, tc.TestMethodName);
        }

        [Fact]
        public void TestCaseIdDefaultsToZero()
        {
            // Arrange / Act
            TestBuilder tb = new();
            TestCase tc = tb.Build();

            // Assert
            Assert.Equal(0, tc.TestCaseId);
        }

        [Fact]
        public void TestCaseIdCanBeOverridden()
        {
            // Arrange / Act
            TestCase tc = new TestBuilder()
                .AddTestCaseId(1)
                .Build();

            // Assert
            Assert.Equal(1, tc.TestCaseId);
        }

        // May not need below test.
        // Still undecided if this should even be configurable.
        [Fact]
        public void ThrowOnFinallyBlockDefaultsToTrue()
        {
            // Arrange / Act
            TestBuilder tb = new();
            TestCase tc = tb.Build();

            // Assert
            Assert.True(tc.ThrowOnFinallyBlockException);
        }

        [Fact]
        public void TestCasePassedDefaultsToFalse()
        {
            // Arrange / Act
            TestBuilder tb = new();
            TestCase tc = tb.Build();

            // Assert
            Assert.False(tc.Passed);
        }

        [Fact]
        public void TestCasePassedRemainsFalseOnFailure()
        {
            // Arrange
            TestCase tc = new TestBuilder()
                .AddDependencyInstance("Fail")
                .AddTestBlock<ExampleTestBlockWithExecuteArg>()
                .Build();

            // Act
            Assert.Throws<TestCaseException>(() => tc.Execute());

            // Assert
            Assert.False(tc.Passed);
        }

        [Fact]
        public void TestCasePassedTurnsTrueOnSuccessfulExecution()
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
    }
}
