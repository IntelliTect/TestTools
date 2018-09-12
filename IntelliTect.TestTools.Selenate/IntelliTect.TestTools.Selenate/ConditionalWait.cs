using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.Selenate
{
    public class ConditionalWait
    {
        public Task<T1> WaitForSeconds<T1, T2>(Func<T1> action, int seconds = 15)
            where T2 : Exception
        {
            return Test(action, seconds, typeof(T2));
        }

        public Task<bool> WaitForSeconds<T>(Action action, int seconds = 15)
            where T : Exception
        {
            return ExecuteWait(action, seconds, typeof(T));
        }

        public Task<bool> WaitForSeconds<T1, T2>(Action action, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2));
        }

        public Task<bool> WaitForSeconds<T1, T2, T3>(Action action, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2), typeof(T3));
        }

        public Task<bool> WaitForSeconds<T1, T2, T3, T4>(Action action, int seconds = 15)
            where T1 : Exception
            where T2 : Exception
            where T3 : Exception
            where T4 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        private async Task<bool> ExecuteWait(Action actionToWaitForComplete, int seconds, params Type[] types)
        {
            DateTime endTime = new DateTime();
            List<Type> typesToCheck = types.ToList();
            List<Exception> exceptions = null;
            endTime.AddSeconds(seconds);
            do
            {
                try
                {
                    actionToWaitForComplete();
                    return true;
                }
                catch (Exception ex) when (types.ToList().Contains(ex.GetType()))
                {
                    exceptions.Add(ex);
                }
                await Task.Delay(250);
            } while (DateTime.Now < endTime);
            throw new AggregateException(exceptions);
        }

        private async Task<T> Test<T>(Func<T> actionToWaitForComplete, int seconds, params Type[] types)
        {
            DateTime endTime = new DateTime();
            List<Type> typesToCheck = types.ToList();
            List<Exception> exceptions = null;
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
    }
}
