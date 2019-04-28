using System.Diagnostics;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class TestBlock1
    {
        public TestBlock1ReturnValues Execute()
        {
            Debug.WriteLine($"Testing some stuff... This test block passes! :) ");
            return new TestBlock1ReturnValues { ActualResult = false };
        }
    }
}
