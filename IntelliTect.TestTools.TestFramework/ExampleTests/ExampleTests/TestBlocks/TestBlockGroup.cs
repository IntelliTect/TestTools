using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class TestBlockGroup
    {
        public TestBlock1ReturnValues TestBlock1()
        {
            Debug.WriteLine($"Testing some stuff... This test block passes! :) ");
            return new TestBlock1ReturnValues { ActualResult = false }; ;
        }

        public void TestBlock2(TestBlock2ExpectedValues expected, TestBlock1ReturnValues actual)
        {
            Debug.WriteLine($"Testing some more stuff... This test block probably fails. :( ");
            Assert.Equal(expected.ExpectedResult, actual.ActualResult);
        }
    }
}
