using System.Diagnostics;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class TestBlock2
    {
        public void Execute(TestBlock2ExpectedValues expected, TestBlock1ReturnValues actual)
        {
            Debug.WriteLine($"Testing some more stuff... This test block probably fails. :( ");
            Assert.Equal(expected.ExpectedResult, actual.ActualResult);
        }
    }

    public class TestBlock2ExpectedValues
    {
        public bool ExpectedResult { get; set; }
    }

    public class TestBlock1ReturnValues
    {
        public bool ActualResult { get; set; }
    }
}
