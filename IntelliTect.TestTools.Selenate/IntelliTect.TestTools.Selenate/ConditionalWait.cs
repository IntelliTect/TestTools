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
        /// <summary>
        /// Repeatedly chekcs for a condition until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <param name="timeToWait"></param>
        /// <param name="exceptionsToIgnore"></param>
        /// <returns></returns>
        public Task<TResult> WaitFor<TResult>(Func<TResult> func, TimeSpan timeToWait, params Type[] exceptionsToIgnore)
        {
            if(exceptionsToIgnore.Any(t => t.GetType() != typeof(Exception)))
            {
                throw new ArgumentException("Invalid type passed into exceptionsToIgnore paramter. Must be of type Exception.");
            }
            return ExecuteWait(func, timeToWait, exceptionsToIgnore);
        }

        public Task WaitFor(Action action, TimeSpan timeToWait, params Type[] exceptionsToIgnore)
        {
            if (exceptionsToIgnore.Any(t => t.GetType() != typeof(Exception)))
            {
                throw new ArgumentException("Invalid type passed into exceptionsToIgnore paramter. Must be of type Exception.");
            }
            return ExecuteWait(action, timeToWait, exceptionsToIgnore);
        }

        public Task<TResult> WaitFor<TException, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException));
        }
        public Task WaitFor<TException>(Action action, TimeSpan timeToWait)
            where TException : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException));
        }

        public Task<TResult> WaitFor<TException1, TException2, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException1), typeof(TException2));
        }

        public Task WaitFor<TException1, TException2>(Action action, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException1), typeof(TException2));
        }

        public Task<TResult> WaitFor<TException1, TException2, TException3, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3));
        }

        public Task WaitFor<TException1, TException2, TException3>(Action action, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3));
        }

        public Task<TResult> WaitFor<TException1, TException2, TException3, TException4, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3), typeof(TException4));
        }

        public Task WaitFor<TException1, TException2, TException3, TException4>(Action action, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3), typeof(TException4));
        }

        private async Task<TResult> ExecuteWait<TResult>(Func<TResult> actionToWaitForComplete, TimeSpan timeToWait, params Type[] types)
        {
            DateTime endTime = DateTime.Now.Add(timeToWait);
            List<Exception> exceptions = new List<Exception>();
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
            throw new AggregateException(exceptions);
        }

        private async Task ExecuteWait(Action actionToWaitForComplete, TimeSpan timeToWait, params Type[] types)
        {
            DateTime endTime = DateTime.Now.Add(timeToWait);
            List<Exception> exceptions = new List<Exception>();
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
            throw new AggregateException(exceptions);
        }
    }
}
