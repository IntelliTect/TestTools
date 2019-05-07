using Newtonsoft.Json;
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
            AddDataToBag(default(T));
            return this;
        }

        public TestBuilder AddData(object data)
        {
            AddDataToBag(data);
            return this;
        }

        public TestBuilder AddData(params object[] data)
        {
            foreach(var d in data)
            {
                AddDataToBag(d);
            }
            return this;
        }

        public void ExecuteTestBlock()
        {
            // Check for non-existent types here
            foreach (Type tb in TestBlockTypes)
            {
                MethodInfo execute = tb.GetMethod("Execute");
                // Clean this code up...

                ConstructorInfo[] ctors = tb.GetConstructors();
                // Assume only one for now. Expand on later.
                ParameterInfo[] ctorArgs = ctors.First().GetParameters();

                object[] args = new object[ctorArgs.Length];
                for (int i = 0; i < ctorArgs.Length; i++)
                {
                    // Super dirty. Clean this up.
                    var t = ctorArgs[i].ParameterType;
                    var p = Data.SingleOrDefault(d => d.GetType() == t);
                    if (p == null)
                        p = Data.SingleOrDefault(d => d.GetType().BaseType == t);
                    if (p == null)
                        // This will produce unexpected results if we load up two different browser types. It will grab whatever is first.
                        p = Data.Single(d => d.GetType().GetInterfaces().ToList().Contains(t));
                    args[i] = p;
                }

                object tbInstance = ctors.First().Invoke(args);
                //Don't convert in-line...
                //Log.Info($"Creating test block with following inputs {JsonConvert.SerializeObject(args)}");
                // Clean this code up...
                ParameterInfo[] argInfo = execute.GetParameters();

                args = new object[argInfo.Length];
                for (int i = 0; i < argInfo.Length; i++)
                {
                    var t = argInfo[i].ParameterType;
                    var p = Data.Single(d => d.GetType() == t);
                    args[i] = p;
                }

                // Need to do this first, up above. Probably can't group finding all of the constructor params
                // and properties
                //ConstructorInfo ctor = tb.GetConstructor(Type.EmptyTypes);
                //object tbInstance = ctor.Invoke(new object[] {});
                //object tbInstance = ctors.First().Invoke(new object[] { });


                Log.Info($"Starting test block {tb.Name}");
                Log.Info($"Using additional inputs {JsonConvert.SerializeObject(args, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })}");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                object result = execute.Invoke(tbInstance, args);
                sw.Stop();
                Log.Info($"Time for test block to execute: {sw.Elapsed}");
                if (result != null)
                {
                    Log.Info($"Test block returned... {JsonConvert.SerializeObject(result, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })}");
                    AddDataToBag(result);
                }
            }
        }

        private void AddDataToBag(params object[] data)
        {
            foreach (var d in data)
            {
                var existingType = Data.SingleOrDefault(f => f.GetType() == d.GetType());
                if (existingType != null)
                {
                    Data.Remove(existingType);
                }
                Data.Add(d);
            }
        }

        private List<Type> TestBlockTypes { get; set; } = new List<Type>();
        private List<object> Data { get; set; } = new List<object>();
        private Log Log { get; set; } = new Log();
    }
}
