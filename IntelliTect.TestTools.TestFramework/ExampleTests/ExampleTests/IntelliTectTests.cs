using ExampleTests.TestBlocks;
using IntelliTect.TestTools.TestFramework;
using System.Reflection;
using Xunit;

namespace ExampleTests
{
    public class IntelliTectTests
    {
        private void Test(MethodInfo method)
        {

        }

        [Fact]
        public void Test1()
        {

            // Ideal scenario...
            //bool r1 = testing.TestBlock1();
            //testing.TestBlock2(false, r1);


            TestBlockGroup testing = new TestBlockGroup();

            Test(testing.TestBlock1);

            var expectedResult = new TestBlock2ExpectedValues { ExpectedResult = true };
            TestBuilder builder = new TestBuilder();
            //builder.AddTestBlock(testing.TestBlock1);
            //builder.AddTestBlock(
            //    () => { return testing.TestBlock1(); },
            //    () => {
            //        testing.TestBlock2(new TestBlock2ExpectedValues { ExpectedResult = true }, new TestBlock2ActualValues { ActualResult = r1 });
            //        return false; }
            //).ExecuteTestByDelegate();

            //builder
            //    .AddTestBlock(testing.TestBlock1)
            //    .AddTestBlock(() => { testing.TestBlock2(expectedResult, actualResult); return false; })
            //    .ExecuteTestByDelegate();

            //builder
            //    .AddTestBlock(() => {
            //        r1 = testing.TestBlock1(); })
            //    .AddTestBlock(() => {
            //        testing.TestBlock2(new TestBlock2ExpectedValues { ExpectedResult = true }, new TestBlock2ActualValues { ActualResult = r1 }); })
            //    .ExecuteTestByDelegate();

            builder
                .AddTestBlock<TestBlocks.TestBlock1>()
                .AddData(expectedResult)
                .AddTestBlock<TestBlocks.TestBlock2>()
                .ExecuteTestBlock();
        }
    }
}
