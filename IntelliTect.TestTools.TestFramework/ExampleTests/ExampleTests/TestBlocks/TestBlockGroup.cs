using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class TestBlockGroup
    {
        public bool TestBlock1()
        {
            Debug.WriteLine($"Testing some stuff... This test block passes! :) ");
            return false;
        }

        public void TestBlock2(TestBlock2ExpectedValues expected, TestBlock2ActualValues actual)
        {
            Debug.WriteLine($"Testing some more stuff... This test block probably fails. :( ");
            Assert.Equal(expected.ExpectedResult, actual.ActualResult);
        }
    }
}
