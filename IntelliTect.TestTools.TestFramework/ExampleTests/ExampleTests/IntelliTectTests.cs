using ExampleTests.TestBlocks;
using IntelliTect.TestTools.TestFramework;
using Xunit;

namespace ExampleTests
{
    public class IntelliTectTests
    {
        [Fact]
        public void Test1()
        {
            // Ideal scenario...
            //bool r1 = testing.TestBlock1();
            //testing.TestBlock2(false, r1);


            // Backup scenario... although at this point, probably easier to just go by type
            TestBuilder builder = new TestBuilder();
            //builder.AddTestBlock(
            //    () => { r1 = testing.TestBlock1(); },
            //    () => { testing.TestBlock2(false, r1); }
            //);

            //builder
            //    .AddTestBlock(() => { r1 = testing.TestBlock1(); })
            //    .AddTestBlock(() => { testing.TestBlock2(false, r1); })
            //    .ExecuteTest();

            builder
                .AddTestBlock<TestBlocks.TestBlock1>()
                .AddData(new TestBlock2ExpectedValues { ExpectedResult = true }, new TestBlock2ActualValues { ActualResult = false })
                .AddTestBlock<TestBlocks.TestBlock2>()
                .ExecuteTest();
        }
    }
}
