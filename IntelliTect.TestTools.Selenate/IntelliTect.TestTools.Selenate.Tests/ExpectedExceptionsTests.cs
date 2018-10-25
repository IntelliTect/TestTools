using System;
using System.Threading.Tasks;
using Xunit;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class ExpectedExceptionsTests : BaseTest
    {
        [Fact]
        public async Task CheckActionsParamsForBaseTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(TestVoidException, TimeSpan.FromSeconds(1), typeof(Exception)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(Exception));
        }

        [Fact]
        public async Task CheckFuncParamsForBaseTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(TestReturnException, TimeSpan.FromSeconds(1), typeof(Exception)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(Exception));
        }

        [Fact]
        public async Task CheckActionParamsForMixedTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until(TestVoidException, TimeSpan.FromSeconds(1), typeof(Exception), typeof(NullReferenceException)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(Exception));
            // Verify that we only catch the actual exceptions thrown
            Assert.DoesNotContain(ae.InnerExceptions, e => e.GetType() == typeof(NullReferenceException));
        }

        [Fact]
        public async Task CheckFuncParamsForMixedTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(
                () => Wait.Until(TestReturnException, TimeSpan.FromSeconds(1), typeof(Exception), typeof(NullReferenceException)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(Exception));
            // Verify that we only catch the actual exceptions thrown
            Assert.DoesNotContain(ae.InnerExceptions, e => e.GetType() == typeof(NullReferenceException));
        }

        [Fact]
        public async Task CheckActionsParamsForOneDerivedTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(() => CheckExceptionsVoidReturn(1, 1), TimeSpan.FromSeconds(1), typeof(InvalidOperationException)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
        }

        [Fact]
        public async Task CheckFuncParamsForOneDerivedTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until(() => CheckExceptionsBoolReturn(1, 1), TimeSpan.FromSeconds(1), typeof(InvalidOperationException)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
        }

        [Fact]
        public async Task CheckActionsGenericForOneDerivedTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException>(() => CheckExceptionsVoidReturn(1, 1), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
        }

        [Fact]
        public async Task CheckFuncGenericForOneDerivedTypeChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException>(() => CheckExceptionsBoolReturn(1, 1), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
        }

        [Fact]
        public async Task CheckActionsGenericForTwoDerivedTypesChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException, InvalidProgramException>(() => CheckExceptionsVoidReturn(1, 2), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
        }

        [Fact]
        public async Task CheckFuncGenericForTwoDerivedTypesChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException, InvalidProgramException>(() => CheckExceptionsBoolReturn(1, 2), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
        }

        [Fact]
        public async Task CheckActionsGenericForThreeDerivedTypesChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => CheckExceptionsVoidReturn(1, 3), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(IndexOutOfRangeException));
        }

        [Fact]
        public async Task CheckFuncGenericForThreeDerivedTypesChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException>(() => CheckExceptionsBoolReturn(1, 3), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(IndexOutOfRangeException));
        }

        [Fact]
        public async Task CheckActionsGenericForFourDerivedTypesChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException, ArgumentNullException>(() => CheckExceptionsVoidReturn(1, 4), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(IndexOutOfRangeException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(ArgumentNullException));
        }

        [Fact]
        public async Task CheckFuncGenericForFourDerivedTypesChecking()
        {
            AggregateException ae = await Assert.ThrowsAsync<AggregateException>(() => Wait.Until<InvalidOperationException, InvalidProgramException, IndexOutOfRangeException, ArgumentNullException>(() => CheckExceptionsBoolReturn(1, 4), TimeSpan.FromSeconds(1)));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidOperationException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(InvalidProgramException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(IndexOutOfRangeException));
            Assert.Contains(ae.InnerExceptions, e => e.GetType() == typeof(ArgumentNullException));
        }
    }
}
