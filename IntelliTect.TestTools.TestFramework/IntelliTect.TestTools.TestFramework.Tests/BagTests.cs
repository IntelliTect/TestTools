using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class BagTests
    {
        [Fact]
        public void CanAddItemToBagByType()
        {
            TestBuilder builder = new TestBuilder();
            builder.AddData<TestData>();

            Assert.True(builder.TryGetItemFromBag(typeof(TestData), out _));
        }
    }

    public class TestData
    {
        public bool IsValid { get; set; }
    }
}
