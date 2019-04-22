using System.Diagnostics;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class TestBlock2
    {
        public void Execute(bool expected, bool actual)
        {
            Debug.WriteLine($"Testing some more stuff... This test block probably fails. :( ");
            Assert.Equal(expected, actual);
        }
    }
}
