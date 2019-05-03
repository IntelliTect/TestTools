using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestBuilder
    {
        // DON'T FORGET TO UNIT TEST ONCE YOU'RE BACK ON YOUR NORMAL MACHINE
        public TestBuilder AddTestBlock<T>()
        {
            TestBlockTypes.Add(typeof(T));
            return this;
        }

        // Probably change this to a factory pattern?
        public TestBuilder AddData<T>()
        {
            Data.Add(default(T));
            return this;
        }

        public TestBuilder AddData(object data)
        {
            AddDataToBag(data);
            return this;
        }

        public TestBuilder AddData(params object[] data)
        {
            Data.AddRange(data);
            return this;
        }

        public void ExecuteTestBlock()
        {
            // Check for non-existent types here
            foreach (Type tb in TestBlockTypes)
            {
                MethodInfo execute = tb.GetMethod("Execute");
                
                // Clean this code up...
                List<ParameterInfo> argInfo = execute.GetParameters().ToList();
                ConstructorInfo[] ctors = tb.GetConstructors();
                // Assume only one for now. Expand on later.
                argInfo.AddRange(ctors.First().GetParameters().ToList());

                object[] args = new object[argInfo.Count];
                for (int i = 0; i < argInfo.Count; i++)
                {
                    var t = argInfo[i].ParameterType;
                    var p = Data.Single(d => d.GetType() == t);
                    args[i] = p;
                }

                // Need to do this first, up above. Probably can't group finding all of the constructor params
                // and properties
                //ConstructorInfo ctor = tb.GetConstructor(Type.EmptyTypes);
                //object tbInstance = ctor.Invoke(new object[] {});
                object tbInstance = ctors.First().Invoke(new object[] { });


                Debug.WriteLine($"Starting test block {tb.Name}");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                object result = execute.Invoke(tbInstance, args);
                sw.Stop();
                Debug.WriteLine($"Time for test block to execute: {sw.Elapsed}");
                if(result != null)
                {
                    Data.Add(result);
                }
            }
        }

        private void AddDataToBag(params object[] data)
        {
            foreach(var d in data)
            {
                var existingType = Data.SingleOrDefault(f => f.GetType() == d.GetType());
                if(existingType != null)
                {
                    Data.Remove(existingType);
                }
                Data.Add(d);
            }
        }

        private List<Type> TestBlockTypes { get; set; } = new List<Type>();
        private List<object> Data { get; set; } = new List<object>();
    }
}
