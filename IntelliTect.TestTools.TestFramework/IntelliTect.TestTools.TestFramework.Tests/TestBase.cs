using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBase
    {
        protected static void ValidateAggregateException(AggregateException result, int expectedInnerExceptions)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            Assert.Equal(expectedInnerExceptions, result.InnerExceptions.Count);
#pragma warning restore CA1062 // Validate arguments of public methods

            foreach (Exception? ie in result.InnerExceptions)
            {
                Assert.Equal(typeof(InvalidOperationException), ie.GetType());
                Assert.Contains(
                    ErrorMesssages.MissingInputError,
                    ie.Message,
                    StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
