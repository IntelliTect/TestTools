using ExampleTests.TestBlocks;
using IntelliTect.TestTools.TestFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace ExampleTests
{
    public class IntelliTectTests
    {
        [Fact]
        public void Test1()
        {
            var expectedResult = new TestBlock2ExpectedValues { ExpectedResult = true };

            // Is there a better way to do this to show the inputs and outputs of test blocks?

            TestBuilder builder = new TestBuilder();
            builder
                .AddTestBlock<TestBlocks.TestBlock1>()
                .AddData(expectedResult)
                .AddTestBlock<TestBlocks.TestBlock2>()
                .ExecuteTestBlock();
        }
    }
}
