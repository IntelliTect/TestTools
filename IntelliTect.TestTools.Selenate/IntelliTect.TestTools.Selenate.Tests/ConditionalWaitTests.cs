using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ConditionalWaitTests
    {
        [Fact]
        public async Task CheckActionParamsForInvalidTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal("Invalid type passed into exceptionsToIgnore parameter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public async Task CheckFuncParamsForInvalidTypeChecking()
        {
            Exception ex = await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(string)));
            Assert.Equal("Invalid type passed into exceptionsToIgnore parameter. Must be of type Exception.", ex.Message);
        }

        [Fact]
        public async Task CheckActionParamsForMixedInvalidTypeChecking()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(Exception), typeof(string)));
        }

        [Fact]
        public async Task CheckFuncParamsForMixedInvalidTypeChecking()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(string), typeof(Exception)));
        }

        [Fact]
        public async Task CheckActionParamsForExpectedExceptions()
        {
            await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(ThrowNullRefException, TimeSpan.FromSeconds(1), typeof(NullReferenceException)));
        }

        [Fact]
        public async Task CheckFuncParamsForExpectedExceptions()
        {
            await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(ThrowNullRefException, TimeSpan.FromSeconds(1), typeof(NullReferenceException)));
        }

        [Fact]
        public async Task CheckActionsParamsForBaseTypeChecking()
        {
            await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(Exception)));
        }

        [Fact]
        public async Task CheckFuncParamsForBaseTypeChecking()
        {
            await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(Exception)));
        }

        [Fact]
        public async Task CheckActionParamsForMixedTypeChecking()
        {
            await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(Exception), typeof(NullReferenceException)));
        }

        [Fact]
        public async Task CheckFuncParamsForMixedTypeChecking()
        {
            await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(Exception), typeof(NullReferenceException)));
        }

        // Might need explicit scenarios for both func and action scenarios in the below examples.
        // Gauge based on how much Wait needs to change based on implementation.
        // Might also need some additional testing around making sure Wait.Until actually times out when expected

        [Fact]
        public async Task CheckForUnexpectedExceptionType()
        {
            await Assert.ThrowsAsync<NullReferenceException>(
                () => Wait.Until<ArgumentNullException>(
                    () => ThrowNullRefException(), TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task CheckForAggregateExceptionFromExpectedExceptionType()
        {
            await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<NullReferenceException>(
                    () => ThrowNullRefException(), TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task CheckForExpectedConditionDoesNotThrowException()
        {
            await Wait.Until<EqualException>(() => Equals(5, 5), TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task CheckForExpectedConditionDoesNotThrowAfterPriorFailuresWithinTimeout()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await Wait.Until<InvalidOperationException>(() => IterateResults(2, 1), TimeSpan.FromSeconds(3));
            sw.Stop();
            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3), "Timeout was reached when it wasn't expected to be");
        }

        [Fact]
        public async Task CheckForTwoExpectedFailuresByParams()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until(() => IterateResults(5, 2), TimeSpan.FromSeconds(2), typeof(InvalidOperationException), typeof(InvalidProgramException)));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
        }

        [Fact]
        public async Task CheckForTwoExpectedFailuresByGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException>(() => IterateResults(5, 2), TimeSpan.FromSeconds(2)));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
        }

        // Might need some more testing around the three and four exception scenarios

        [Fact]
        public async Task CheckForThreeExpectedFailuresWithGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => IterateResults(5, 3), TimeSpan.FromSeconds(2)));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(IndexOutOfRangeException));
        }

        [Fact]
        public async Task CheckForFourExpectedFailuresWithGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException, ArgumentNullException>(() => IterateResults(5, 4), TimeSpan.FromSeconds(2)));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(IndexOutOfRangeException));
            Assert.Contains(ex.InnerExceptions, e => e.GetType() == typeof(ArgumentNullException));
        }

        [Fact]
        public async Task CheckForMultipleFailuresByGenericOneUnexpected()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => IterateResults(5, 4), TimeSpan.FromSeconds(2)));
        }

        [Fact]
        public async Task CheckForMultipleFailuresByParamsOneUnexpected()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => Wait.Until(() => IterateResults(5, 4), TimeSpan.FromSeconds(2), typeof(InvalidOperationException), typeof(InvalidProgramException), typeof(IndexOutOfRangeException)));
        }

        private void TestVoidDelegate()
        {
            throw new Exception();
        }

        private bool TestReturnDelegate()
        {
            throw new Exception();
        }

        private void ThrowNullRefException()
        {
            throw new NullReferenceException();
        }

        private void Equals(int expected, int value)
        {
            if (value == expected)
            {
                return;
            }
            throw new EqualException(expected, value);
        }

        private void IterateResults(int secondsToFail = 1, int numberOfDifferentExceptions = 1)
        {
            if (_End == DateTime.MinValue)
            {
                _End = DateTime.Now.AddSeconds(secondsToFail);
            }

            while (DateTime.Now < _End)
            {
                _Attempts++;
                if (_Attempts > numberOfDifferentExceptions)
                {
                    _Attempts = 1;
                }
                switch (_Attempts)
                {
                    case 1:
                        throw new InvalidOperationException();
                    case 2:
                        throw new InvalidProgramException();
                    case 3:
                        throw new IndexOutOfRangeException();
                    case 4:
                        throw new ArgumentNullException();
                    default:
                        throw new ArgumentException();
                }
            }
        }

        private int _Attempts = 0;
        private DateTime _End = DateTime.MinValue;
    }
}
