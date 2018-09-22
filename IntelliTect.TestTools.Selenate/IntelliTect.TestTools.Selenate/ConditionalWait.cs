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
        public Task<TResult> WaitForSeconds<T1, TResult>(Func<TResult> func, int seconds = 15)
            where T1 : Exception
        {
            return ExecuteWait(func, seconds, typeof(T1));
        }
        public Task WaitForSeconds<T1>(Action action, int seconds = 15)
            where T1 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1));
        }

        public Task<TResult> WaitForSeconds<T1, T2, TResult>(Func<TResult> func, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
        {
            return ExecuteWait(func, seconds, typeof(T1), typeof(T2));
        }

        public Task WaitForSeconds<T1, T2>(Action action, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2));
        }

        public Task<TResult> WaitForSeconds<T1, T2, T3, TResult>(Func<TResult> func, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
        {
            return ExecuteWait(func, seconds, typeof(T1), typeof(T2), typeof(T3));
        }

        public Task WaitForSeconds<T1, T2, T3>(Action action, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2), typeof(T3));
        }

        public Task<TResult> WaitForSeconds<T1, T2, T3, T4, TResult>(Func<TResult> func, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
            where T4 : Exception
        {
            return ExecuteWait(func, seconds, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public Task WaitForSeconds<T1, T2, T3, T4>(Action action, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
            where T4 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        private async Task<TResult> ExecuteWait<TResult>(Func<TResult> actionToWaitForComplete, int seconds, params Type[] types)
        {
            DateTime endTime = DateTime.Now.AddSeconds(seconds);
            List<Exception> exceptions = new List<Exception>();
            endTime.AddSeconds(seconds);
            do
            {
                try
                {
                    return actionToWaitForComplete();
                }
                catch (Exception ex) when (types.ToList().Contains(ex.GetType()))
                {
                    exceptions.Add(ex);
                }
                await Task.Delay(250);
            } while (DateTime.Now < endTime);
            throw new AggregateException(exceptions);
        }

        private async Task ExecuteWait(Action actionToWaitForComplete, int seconds, params Type[] types)
        {
            DateTime endTime = new DateTime();
            List<Exception> exceptions = null;
            endTime.AddSeconds(seconds);
            do
            {
                try
                {
                    actionToWaitForComplete();
                    return;
                }
                catch (Exception ex) when (types.ToList().Contains(ex.GetType()))
                {
                    exceptions.Add(ex);
                }
                await Task.Delay(250);
            } while (DateTime.Now < endTime);
            throw new AggregateException(exceptions);
        }
    }
}
