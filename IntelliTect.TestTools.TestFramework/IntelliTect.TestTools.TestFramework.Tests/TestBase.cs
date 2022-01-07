using IntelliTect.TestTools.TestFramework.Tests.TestData;
using System;
using System.Linq;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBase
    {
        protected static void ValidateAggregateException(AggregateException result, int expectedInnerExceptions, params string[] messages)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            Assert.Equal(expectedInnerExceptions, result.InnerExceptions.Count);
#pragma warning restore CA1062 // Validate arguments of public methods

            foreach(string s in messages)
            {
                Assert.Contains(result.InnerExceptions,
                    m => m.Message.Contains(
                        s,
                        StringComparison.InvariantCultureIgnoreCase));
            }

            //foreach (Exception? ie in result.InnerExceptions)
            //{
            //    Assert.Equal(typeof(InvalidOperationException), ie.GetType());
            //    // Do we need to verify that every single error message gets triggered?
            //    Assert.Contains(
            //        messages, 
            //        m => m.Contains(
            //            ie.Message,
            //            StringComparison.InvariantCultureIgnoreCase));
            //    //Assert.Contains(
            //    //    messages,
            //    //    ie.Message,
            //    //    StringComparison.InvariantCultureIgnoreCase);
            //}
        }
    }
}
