using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.Selenate
{
    public static class Wait
    {
        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TResult">Return type of the function to evaluate</typeparam>
        /// <param name="func">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <param name="exceptionsToIgnore">A list of exceptions to ignore when attempting to evaluate the function</param>
        /// <returns>An async task that can return a value of type TResult</returns>
        public static Task<TResult> Until<TResult>(Func<TResult> func, TimeSpan timeToWait, params Type[] exceptionsToIgnore)
        {
            CheckParams(exceptionsToIgnore);
            return ExecuteWait(func, timeToWait, exceptionsToIgnore);
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <param name="action">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <param name="exceptionsToIgnore">A list of exceptions to ignore when attempting to evaluate the function</param>
        /// <returns>An async task for the operation</returns>
        public static Task Until(Action action, TimeSpan timeToWait, params Type[] exceptionsToIgnore)
        {
            CheckParams(exceptionsToIgnore);
            return ExecuteWait(action, timeToWait, exceptionsToIgnore);
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TResult">Return type of the function to evaluate</typeparam>
        /// <param name="func">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task that can return a value of type TResult</returns>
        public static Task<TResult> Until<TException, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <param name="action">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task for the operation</returns>
        public static Task Until<TException>(Action action, TimeSpan timeToWait)
            where TException : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException1">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException2">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TResult">Return type of the function to evaluate</typeparam>
        /// <param name="func">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task that can return a value of type TResult</returns>
        public static Task<TResult> Until<TException1, TException2, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException1), typeof(TException2));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException1">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException2">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <param name="action">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task for the operation</returns>
        public static Task Until<TException1, TException2>(Action action, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException1), typeof(TException2));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException1">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException2">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException3">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TResult">Return type of the function to evaluate</typeparam>
        /// <param name="func">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task that can return a value of type TResult</returns>
        public static Task<TResult> Until<TException1, TException2, TException3, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException1">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException2">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException3">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <param name="action">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task for the operation</returns>
        public static Task Until<TException1, TException2, TException3>(Action action, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException1">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException2">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException3">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException4">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TResult">Return type of the function to evaluate</typeparam>
        /// <param name="func">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task that can return a value of type TResult</returns>
        public static Task<TResult> Until<TException1, TException2, TException3, TException4, TResult>(Func<TResult> func, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            return ExecuteWait(func, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3), typeof(TException4));
        }

        /// <summary>
        /// Repeatedly checks for a condition with void return type until it is satisifed or a timeout is reached
        /// </summary>
        /// <typeparam name="TException1">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException2">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException3">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <typeparam name="TException4">An exception type to ignore when attempting to evaluate the function</typeparam>
        /// <param name="action">Function to check for valid evaluation</param>
        /// <param name="timeToWait">Time to try evaluating the given function until an exception is thrown</param>
        /// <returns>An async task for the operation</returns>
        public static Task Until<TException1, TException2, TException3, TException4>(Action action, TimeSpan timeToWait)
            where TException1 : Exception
            where TException2 : Exception
            where TException3 : Exception
            where TException4 : Exception
        {
            return ExecuteWait(action, timeToWait, typeof(TException1), typeof(TException2), typeof(TException3), typeof(TException4));
        }

        private static async Task<TResult> ExecuteWait<TResult>(Func<TResult> actionToWaitForComplete, TimeSpan timeToWait, params Type[] types)
        {
            DateTime endTime = DateTime.Now.Add(timeToWait);
            List<Exception> exceptions = new List<Exception>();
            do
            {
                try
                {
                    return actionToWaitForComplete();
                }
                catch (Exception ex) when (types.Contains(ex.GetType()))
                {
                    exceptions.Add(ex);
                }
                await Task.Delay(250);
            } while (DateTime.Now < endTime);
            throw new AggregateException(exceptions);
        }

        private static async Task ExecuteWait(Action actionToWaitForComplete, TimeSpan timeToWait, params Type[] types)
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

        private static void CheckParams(params Type[] exes)
        {
            if (!exes.Any(e => e.IsSubclassOf(typeof(Exception)) || e.UnderlyingSystemType == typeof(Exception)))
            {
                throw new ArgumentException("Invalid type passed into exceptionsToIgnore parameter. Must be of type Exception.");
            }
        }
    }
}
