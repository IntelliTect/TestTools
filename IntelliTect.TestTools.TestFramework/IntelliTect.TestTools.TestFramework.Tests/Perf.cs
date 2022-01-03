﻿using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class Perf
    {
        private readonly long _Iterations = 100000000;
        private readonly HashSet<object> _HashSet = new();
        private readonly HashSet<int> _PopulatedHashSet = new(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        //private object[] _Array = Array.Empty<object>();
        private readonly int[] _PopulatedArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly List<object> _List = new();
        private readonly List<int> _PopulatedList = new() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly Dictionary<int, int> _PopulatedDictionary = new() { { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 }, { 6, 6 }, { 7, 7 }, { 8, 8 }, { 9, 9 }, { 10, 10 } };
        private readonly Random _Rand = new();

        // Next test: Profile each collection type with foreach and for loops

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
            // So far, this is faster than FirstOrDefault calls, and List.BinarySearch.
            // Fetching by index on an array is faster, but that presumes we know the index.
            IterateOverAction(i => _PopulatedHashSet.TryGetValue(i, out _));
        }

        [Fact]
        public void TestSearchThroughArray()
        {
            IterateOverAction(i => _ = _PopulatedArray[i]);
        }

        [Fact]
        public void TestSearchThroughList()
        {
            IterateOverAction(i =>
            {
                int index = _PopulatedList.BinarySearch(i);
                _ = _PopulatedList[index];
            });
        }

        [Fact]
        public void TestSearchThroughDictionary()
        {
            IterateOverAction(i => _PopulatedDictionary.TryGetValue(i, out _));
        }

        [Fact]
        public void Test()
        {
            Dictionary<Type, object> test = new() { { typeof(string), "x" } };
            if(!test.TryGetValue(typeof(int), out object? obj))
            {
                Assert.Equal(10, obj);
            }
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
                int r = _Rand.Next(0, 10);
                action(r);
            }
        }
    }
}
