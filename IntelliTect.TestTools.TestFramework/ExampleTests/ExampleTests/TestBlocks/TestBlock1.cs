using System.Diagnostics;
using Xunit;

namespace ExampleTests.TestBlocks
{
    public class TestBlock1
    {
        public bool Execute()
        {
            Debug.WriteLine($"Testing some stuff... This test block passes! :) ");
            return false;
        }
    }
}
