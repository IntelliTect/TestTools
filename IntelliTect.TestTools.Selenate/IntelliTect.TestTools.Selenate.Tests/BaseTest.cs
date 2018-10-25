using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IntelliTect.TestTools.Selenate.Tests
{
    public class BaseTest
    {
        protected void TestVoidException()
        {
            throw new Exception();
        }

        protected bool TestReturnException()
        {
            throw new Exception();
        }

        protected void TestVoidNullRefException()
        {
            throw new NullReferenceException();
        }

        protected void TestReturnNullRefException()
        {
            throw new NullReferenceException();
        }

        protected void CheckExceptionsVoidReturn(int secondsToFail = 1, int numberOfDifferentExceptions = 1)
        {
            ThrowExceptions(secondsToFail, numberOfDifferentExceptions);
        }

        protected bool CheckExceptionsBoolReturn(int secondsToFail = 1, int numberOfDifferentExceptions = 1)
        {
            ThrowExceptions(secondsToFail, numberOfDifferentExceptions);
            return true;
        }

        private void ThrowExceptions(int secondsToFail, int numberOfExceptions)
        {
            if (_Timeout == TimeSpan.MinValue)
            {
                _Timeout = TimeSpan.FromSeconds(secondsToFail);
                _Sw.Start();
            }

            while (_Sw.Elapsed <= _Timeout)
            {
                _Attempts++;
                if (_Attempts > numberOfExceptions)
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
        private TimeSpan _Timeout = TimeSpan.MinValue;
        private Stopwatch _Sw = new Stopwatch();
    }
}
