using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class Perf
    {
        private readonly int _Iterations = 50000000;
        private HashSet<object> _HashSet = new();
        private HashSet<object> _PopulatedHashSet = new(new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        //private object[] _Array = Array.Empty<object>();
        private object[] _PopulatedArray = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private List<object> _List = new();
        private List<object> _PopulatedList = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        [Fact]
        public void TestWithProvider()
        {
            TestBuilder builder = new();
            builder.AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>();

            IterateOverAction(builder.BuildWithoutProvider);
        }

        [Fact]
        public void TestWithoutProvider()
        {
            TestBuilder builder = new();
            builder.AddDependencyInstance("Testing")
                .AddTestBlock<ExampleTestBlockWithConstructor>();

            IterateOverAction(builder.BuildWithoutProvider);
        }

        [Fact]
        public void TestAddToHashSet()
        {
            IterateOverAction(i => _HashSet.Add(i));
        }

        // This doesn't work for some reason. Seems like maybe a deadlock?
        // Investigate further.
        //[Fact]
        //public void TestAddToArray()
        //{
        //    IterateOverAction(AddToArray);

        //    void AddToArray(int i)
        //    {
        //        object[] temp = _Array;
        //        _Array = temp.Concat(new object[] { i }).ToArray();
        //    }
        //}

        [Fact]
        public void TestAddToList()
        {
            IterateOverAction(i => _List.Add(i));
        }

        [Fact]
        public void TestSearchThroughHashSet()
        {
            // This is actually slower than the other examples.
            // Might be wroth using hashset for 'contains' but then an array or list for fetching?
            //IterateOverAction(i => _PopulatedHashSet.FirstOrDefault(h => h is 5));

            // This is about 3x faster than the above and other FirstOrDefault calls.
            // Logic could be ensuring a HashSet and Array have the same values, then: if in Hashset, only then find from array?
            // Need to figure out the memory implications of that.
            IterateOverAction(i => _PopulatedHashSet.Contains(5));
        }

        [Fact]
        public void TestSearchThroughArray()
        {
            IterateOverAction(i => _PopulatedArray.FirstOrDefault(a => a is 5));
        }

        [Fact]
        public void TestSearchThroughList()
        {
            IterateOverAction(i => _PopulatedList.FirstOrDefault(a => a is 5));
        }

        private void IterateOverAction(Action action)
        {
            for (int i = 0; i < _Iterations; i++)
            {
                action();
            }
        }

        private void IterateOverAction(Action<int> action)
        {
            for (int i = 0; i < _Iterations; i++)
            {
                action(i);
            }
        }
    }
}
