﻿using System;
using Xunit;

namespace IntelliTect.TestTools.TestFramework.Tests
{
    public class TestBase
    {
        protected static void ValidateAggregateException(AggregateException result, int expectedInnerExceptions, params string[] messages)
        {
            ArgumentNullException.ThrowIfNull(nameof(result));
            Assert.Equal(expectedInnerExceptions, result.InnerExceptions.Count);

            foreach(string s in messages)
            {
                Assert.Contains(result.InnerExceptions,
                    m => m.Message.Contains(
                        s,
                        StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}
