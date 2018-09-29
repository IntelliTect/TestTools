using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.Selenate
{
    public class ConditionalWait
    {
        public Task<TResult> WaitForSeconds<TException, TResult>(Func<TResult> func, int seconds = 15)
            where TException : Exception
        {
            return ExecuteWait(func, seconds, typeof(TException));
        }
        public Task WaitForSeconds<TException>(Action action, int seconds = 15)
            where TException : Exception
        {
            return ExecuteWait(action, seconds, typeof(TException));
        }

        public Task<TResult> WaitForSeconds<TException1, TException2, TResult>(Func<TResult> func, int seconds = 15)
            where TException1 : Exception
            where TException2 : Exception
        {
            return ExecuteWait(func, seconds, typeof(TException1), typeof(TException2));
        }

        public Task WaitForSeconds<TException1, TException2>(Action action, int seconds = 15)
            where TException1 : Exception
            where TException2 : Exception
        {
            return ExecuteWait(action, seconds, typeof(TException1), typeof(TException2));
        }

        public Task<TResult> WaitForSeconds<TException1, TException2, TException3, TResult>(Func<TResult> func, int seconds = 15)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            return ExecuteWait(func, seconds, typeof(TException1), typeof(TException2), typeof(TException3));
        }

        public Task WaitForSeconds<TException1, TException2, TException3>(Action action, int seconds = 15)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            return ExecuteWait(action, seconds, typeof(TException1), typeof(TException2), typeof(TException3));
        }

        public Task<TResult> WaitForSeconds<TException1, TException2, TException3, TException4, TResult>(Func<TResult> func, int seconds = 15)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            return ExecuteWait(func, seconds, typeof(TException1), typeof(TException2), typeof(TException3), typeof(TException4));
        }

        public Task WaitForSeconds<TException1, TException2, TException3, TException4>(Action action, int seconds = 15)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            return ExecuteWait(action, seconds, typeof(TException1), typeof(TException2), typeof(TException3), typeof(TException4));
        }

        private async Task<TResult> ExecuteWait<TResult>(Func<TResult> actionToWaitForComplete, int seconds, params Type[] types)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            List<Exception> exceptions = new List<Exception>();
            endTime.AddSeconds(seconds);
            TResult task = default(TResult);
            do
            {
                try
                {
                    task = actionToWaitForComplete();
                }
                catch (Exception ex) when (types.Contains(ex.GetType()))
                {
                    exceptions.Add(ex);
                }
                await Task.Delay(250);
            } while (DateTime.Now < endTime);
            return task;
            //throw new AggregateException(exceptions);
        }

        private async Task ExecuteWait(Action actionToWaitForComplete, int seconds, params Type[] types)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            List<Exception> exceptions = new List<Exception>();
            endTime.AddSeconds(seconds);
            do
            {
                try
                {
                    actionToWaitForComplete();
                    return;
                }
                catch (Exception ex) when (types.Contains(ex.GetType()))
                {
                    exceptions.Add(ex);
                }
                await Task.Delay(250);
            } while (DateTime.Now < endTime);
            return;
            //throw new AggregateException(exceptions);
        }
    }
}
