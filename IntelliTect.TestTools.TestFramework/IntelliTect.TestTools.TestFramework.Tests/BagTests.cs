using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class BagTests
    {
        [Fact]
        public void CanAddItemToBag()
        {
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag(new TestData());

            Assert.True(bag.TryGetItemFromBag(typeof(TestData), out _), 
                $"Did not find an item of type {typeof(TestData)} in the TestObjectsBag when we expected one");
        }
    }

    public class TestData
    {
        public bool IsValid { get; set; }
    }
}
