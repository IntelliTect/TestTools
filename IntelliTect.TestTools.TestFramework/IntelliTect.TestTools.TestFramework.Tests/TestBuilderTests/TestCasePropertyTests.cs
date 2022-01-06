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
            TestCase tc = new TestBuilder()
                .AddTestCaseName(_Name)
                .Build();
            Assert.Equal(_Name, tc.TestCaseName);
        }

        [Fact]
        public void NullTestMethodNameIsOverriddenToGenericTestCaseName()
        {
            TestBuilder tb = new(null);
            TestCase tc = tb.Build();
            Assert.Equal("UndefinedTestMethodName", tc.TestCaseName);
        }

        [Fact]
        public void TestCaseNameIsOverriddenToCallingMethod()
        {
            TestBuilder tb = new();
            TestCase tc = tb.Build();
            Assert.Equal(nameof(TestCaseNameIsOverriddenToCallingMethod), tc.TestCaseName);
        }

        [Fact]
        public void TestCaseNameIsOverriddenByConstructorArg()
        {
            TestBuilder tb = new(_Name);
            TestCase tc = tb.Build();
            Assert.Equal(_Name, tc.TestCaseName);
        }

        [Fact]
        public void NullTestMethodNameIsOverriddenToGenericTestMethodName()
        {
            TestBuilder tb = new(null);
            TestCase tc = tb.Build();
            Assert.Equal("UndefinedTestMethodName", tc.TestMethodName);
        }

        [Fact]
        public void TestMethodNameIsOverriddenToCallingMethod()
        {
            TestBuilder tb = new();
            TestCase tc = tb.Build();
            Assert.Equal(nameof(TestMethodNameIsOverriddenToCallingMethod), tc.TestMethodName);
        }

        [Fact]
        public void TestMethodNameIsOverriddenByConstructorArg()
        {
            TestBuilder tb = new(_Name);
            TestCase tc = tb.Build();
            Assert.Equal(_Name, tc.TestMethodName);
        }

        [Fact]
        public void TestCaseIdDefaultsToZero()
        {
            TestBuilder tb = new();
            TestCase tc = tb.Build();
            Assert.Equal(0, tc.TestCaseId);
        }

        [Fact]
        public void TestCaseIdCanBeOverridden()
        {
            TestCase tc = new TestBuilder()
                .AddTestCaseId(1)
                .Build();
            Assert.Equal(1, tc.TestCaseId);
        }

        [Fact]
        public void TestCasePassedDefaultsToFalse()
        {
            TestBuilder tb = new();
            TestCase tc = tb.Build();
            Assert.False(tc.Passed);
        }

        // May not need below test.
        // Still undecided if this should even be configurable.
        [Fact]
        public void ThrowOnFinallyBlockDefaultsToTrue()
        {
            TestBuilder tb = new();
            TestCase tc = tb.Build();
            Assert.True(tc.ThrowOnFinallyBlockException);
        }
    }
}
