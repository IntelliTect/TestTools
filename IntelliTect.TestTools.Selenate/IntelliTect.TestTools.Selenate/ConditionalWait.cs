using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTect.TestTools.Selenate
{
    public class ConditionalWait
    {
        public bool WaitForSeconds<T>(Action action, int seconds)
            where T : Exception
        {
            return ExecuteWait(action, seconds, typeof(T));
        }

        public bool WaitForSeconds<T1, T2>(Action action, int seconds)
            where T1 : Exception
            where T2 : Exception
        {
            return ExecuteWait(action, seconds, typeof(T1), typeof(T2));
        }

        private bool ExecuteWait(Action actionToWaitForComplete, int seconds, params Type[] types)
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
                Task.Delay(250).Wait();
            } while (DateTime.Now < endTime);
            
            return false;
        }
    }
}
