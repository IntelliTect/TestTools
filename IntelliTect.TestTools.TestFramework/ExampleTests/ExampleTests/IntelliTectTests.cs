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


            TestBlockGroup testing = new TestBlockGroup();
            bool r1 = false;
            TestBuilder builder = new TestBuilder();
            //builder.AddTestBlock(
            //    () => { return testing.TestBlock1(); },
            //    () => {
            //        testing.TestBlock2(new TestBlock2ExpectedValues { ExpectedResult = true }, new TestBlock2ActualValues { ActualResult = r1 });
            //        return false; }
            //).ExecuteTestByDelegate();

            builder
                .AddTestBlock(() => {
                    r1 = testing.TestBlock1(); })
                .AddTestBlock(() => {
                    testing.TestBlock2(new TestBlock2ExpectedValues { ExpectedResult = true }, new TestBlock2ActualValues { ActualResult = r1 }); })
                .ExecuteTestByDelegate();

            builder
                .AddTestBlock<TestBlocks.TestBlock1>()
                .AddData(new TestBlock2ExpectedValues { ExpectedResult = true }, new TestBlock2ActualValues { ActualResult = false })
                .AddTestBlock<TestBlocks.TestBlock2>()
                .ExecuteTestByBlockType();
        }
    }
}
