using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class BagTests
    {
        [Fact]
        public void CanAddSimpleType()
        {
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag("Cthulhu!");

            Assert.True(bag.TryGetItemFromBag(typeof(string), out _),
                $"Did not find an item of type {typeof(TestData)} in the TestObjectsBag when we expected one");
        }

        [Fact]
        public void CanFindSimpleItemByObject()
        {
            string cth = "Cthulhu!";
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag(cth);

            bag.TryGetItemFromBag(cth, out object foundString);

            Assert.Equal(cth, foundString);
        }

        [Fact]
        public void CanFindSimpleItemByType()
        {
            TestData data = new TestData();
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag(data);

            bag.TryGetItemFromBag(typeof(TestData), out object foundData);

            Assert.Equal(data, foundData);
        }

        [Fact]
        public void CanFindItemByType()
        {
            TestData data = new TestData();
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag(data);

            bag.TryGetItemFromBag(data, out object foundData);

            Assert.Equal(data, foundData);
        }

        [Fact]
        public void CanFindItemByObject()
        {
            string cth = "Cthulhu!";
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag(cth);

            bag.TryGetItemFromBag(typeof(string), out object foundString);

            Assert.Equal(cth, foundString);
        }

        [Fact]
        public void CanAddItemToBagByType()
        {
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag<TestData>();

            Assert.True(bag.TryGetItemFromBag(typeof(TestData), out _), 
                $"Did not find an item of type {typeof(TestData)} in the TestObjectsBag when we expected one");
        }

        [Fact]
        public void CanAddItemToBagByObject()
        {
            TestObjectsBag bag = new TestObjectsBag();
            TestData data = new TestData();
            bag.AddItemToBag(data);

            Assert.True(bag.TryGetItemFromBag(data, out _),
                $"Did not find an item of type {typeof(TestData)} in the TestObjectsBag when we expected one");
        }

        [Fact]
        public void TryToFindNonexistantItemByType()
        {
            TestObjectsBag bag = new TestObjectsBag();
            bag.AddItemToBag(new TestData());

            Assert.False(bag.TryGetItemFromBag(typeof(string), out _),
                $"Found an item of type {typeof(string)} in the TestObjectsBag when we were NOT expecting one");
        }

        [Fact]
        public void TryToFindNonexistantItemByObject()
        {
            TestObjectsBag bag = new TestObjectsBag();
            TestData data = new TestData();
            bag.AddItemToBag(data);

            Assert.False(bag.TryGetItemFromBag("Test", out _),
                $"Found an item of type {typeof(string)} in the TestObjectsBag when we were NOT expecting one");
        }
    }

    public class TestData
    {
        public bool IsValid { get; set; }
    }
}
