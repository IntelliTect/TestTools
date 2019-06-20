using ExampleTests.Harness;
using IntelliTect.TestTools.TestFramework;

namespace ExampleTests.TestBlocks
{
    public class TestBlockBase : ITestBlock
    {
        public IntelliTectWebpage IntelliTect { get; set; }
    }
}
