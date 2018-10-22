using System;
using System.Collections.Generic;
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
        public void CheckActionParamsForValidTypeChecking()
        {
            Wait.Until(TestVoidDelegate, TimeSpan.FromSeconds(1), typeof(NullReferenceException));
        }

        [Fact]
        public void CheckFuncParamsForValidTypeChecking()
        {
            Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(NullReferenceException));
        }

        [Fact]
        public void CheckFuncParamsForBaseTypeChecking()
        {
            Wait.Until(TestReturnDelegate, TimeSpan.FromSeconds(1), typeof(Exception));
        }

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
                    () => Equals( 4, 5 ), TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void CheckForExpectedSuccess()
        {
            // If this ever breaks, it should throw an exception
            Wait.Until<EqualException>(() => Equals(5, 5), TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task CheckForSuccessAfterFailure()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            await Wait.Until<InvalidOperationException>(() => IterateResults(true), TimeSpan.FromSeconds(10));
            sw.Stop();
            Assert.True(sw.Elapsed < TimeSpan.FromSeconds(10), "Timeout was reached when it wasn't expected to be");
        }

        [Fact]
        public async Task CheckForMultipleExpectedFailuresByGeneric()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until<InvalidOperationException, InvalidProgramException>(() => IterateResults(false), TimeSpan.FromSeconds(5)));
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidOperationException>());
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidProgramException>());
        }

        [Fact]
        public async Task CheckForMultipleExpectedFailuresByParams()
        {
            AggregateException ex = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until(() => IterateResults(false), TimeSpan.FromSeconds(5), typeof(InvalidOperationException), typeof(InvalidProgramException)));
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidOperationException>());
            Assert.NotNull(ex.InnerExceptions.OfType<InvalidProgramException>());
        }

        // Make sure to also cover scenarios of three and four generic types

        [Fact]
        public void CheckForMultipleFailuresByGenericOneUnexpected()
        {
            Assert.ThrowsAsync<InvalidOperationException>(
                () => Wait.Until<InvalidProgramException>(() => IterateResults(false), TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void CheckForMultipleFailuresByParamsOneUnexpected()
        {
            Assert.ThrowsAsync<InvalidOperationException>(
                () => Wait.Until(() => IterateResults(false), TimeSpan.FromSeconds(5), typeof(InvalidProgramException)));
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

        private void IterateResults(bool passTest)
        {
            if (passTest)
            {
                return;
            }
            _Attempts++;
            if (_Attempts % 2 == 0)
            {
                throw new InvalidOperationException();
            }
            throw new InvalidProgramException();
        }

        private int _Attempts = 0;
    }
}
