using System;
using System.Diagnostics;
using System.Linq;
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
                () => Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(Exception), typeof(object)));
        }

        [Fact]
        public async Task CheckFuncParamsForMixedInvalidTypeChecking()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(object), typeof(Exception)));
        }

        [Fact]
        public async Task CheckActionParamsForValidTypeChecking()
        {
            await Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(NullReferenceException));
        }

        [Fact]
        public async Task CheckFuncParamsForValidTypeChecking()
        {
            await Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(NullReferenceException));
        }

        [Fact]
        public async Task CheckActionsParamsForBaseTypeChecking()
        {
            await Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(Exception));
        }

        [Fact]
        public async Task CheckFuncParamsForBaseTypeChecking()
        {
            await Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(Exception));
        }

        [Fact]
        public async Task CheckActionParamsForMixedTypeChecking()
        {
            await Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(Exception), typeof(NullReferenceException));
        }

        [Fact]
        public async Task CheckFuncParamsForMixedTypeChecking()
        {
            await Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(Exception), typeof(NullReferenceException));
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
        public async Task CheckForExpectedFailure()
        {
            await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<EqualException>(
                    () => Equals(4, 5), TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public async Task CheckForExpectedSuccess()
        {
            await Wait.Until<EqualException>(() => Equals(5, 5), TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task CheckForSuccessAfterFailure()
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
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidOperationException>());
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidProgramException>());
        }

        [Fact]
        public async Task CheckForTwoExpectedFailuresByGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException>(() => IterateResults(5, 2), TimeSpan.FromSeconds(2)));
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidOperationException>());
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidProgramException>());
        }

        // Might need some more testing around the three and four exception scenarios

        [Fact]
        public async Task CheckForThreeExpectedFailuresWithGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => IterateResults(5, 3), TimeSpan.FromSeconds(2)));
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidOperationException>());
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidProgramException>());
            Assert.NotNull(ex.InnerExceptions.OfType<IndexOutOfRangeException>());
        }

        [Fact]
        public async Task CheckForFourExpectedFailuresWithGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException, ArgumentNullException>(() => IterateResults(5, 4), TimeSpan.FromSeconds(2)));
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidOperationException>());
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidProgramException>());
            Assert.NotNull(ex.InnerExceptions.OfType<IndexOutOfRangeException>());
            Assert.NotNull(ex.InnerExceptions.OfType<ArgumentNullException>());
        }

        //[Fact]
        public async Task CheckForMultipleFailuresByGenericOneUnexpected()
        {
            //Assert.ThrowsAsync<InvalidOperationException>(
            //    () => Wait.Until<InvalidProgramException>(() => IterateResults(false, 2, 2), TimeSpan.FromSeconds(5)));
        }

        //[Fact]
        public async Task CheckForMultipleFailuresByParamsOneUnexpected()
        {
            //Assert.ThrowsAsync<InvalidOperationException>(
            //    () => Wait.Until(() => IterateResults(false, 2, 2), TimeSpan.FromSeconds(5), typeof(InvalidProgramException)));
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
                switch (numberOfDifferentExceptions)
                {
                    case 1:
                        CheckFirstException();
                        break;
                    case 2:
                        CheckFirstException();
                        CheckSecondException();
                        break;
                    case 3:
                        CheckFirstException();
                        CheckSecondException();
                        CheckThirdException();
                        break;
                    case 4:
                        CheckFirstException();
                        CheckSecondException();
                        CheckThirdException();
                        throw new ArgumentNullException();
                    default:
                        throw new ArgumentException();
                }
            }

            void CheckFirstException()
            {
                if (_Attempts % 2 == 1)
                {
                    throw new InvalidOperationException();
                }
            }

            void CheckSecondException()
            {
                if (_Attempts % 2 == 0)
                {
                    throw new InvalidProgramException();
                }
            }

            void CheckThirdException()
            {
                if (_Attempts % 3 == 0)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        private int _Attempts = 0;
        private DateTime _End = DateTime.MinValue;
    }
}
