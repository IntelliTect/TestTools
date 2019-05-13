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
        public TestBuilder AddTestBlock<T>()
        {
            TestBlockTypes.Add(typeof(T));
            return this;
        }

        // Probably change this to a factory pattern?
        public TestBuilder AddData<T>()
        {
            ObjectBag.AddItemToBag(default(T));
            return this;
        }

        public TestBuilder AddData(object data)
        {
            ObjectBag.AddItemToBag(data);
            return this;
        }

        public TestBuilder AddData(params object[] data)
        {
            foreach (var d in data)
            {
                ObjectBag.AddItemToBag(data);
            }
            return this;
        }

        public void ExecuteTestCase()
        {
            // Check for non-existent types here?

            // Execute test blocks
            foreach (Type tb in TestBlockTypes)
            {
                // Get constructor and arguments
                Log.Info($"Gathering constructor arguments for test block {tb}...");
                ConstructorInfo[] ctors = tb.GetConstructors();
                ParameterInfo[] ctorArgs = ctors.First().GetParameters();
                object[] args = ValidateAndFetchTestBlockParameters(ctorArgs);

                // Create instance of test block
                object tbInstance = ctors.First().Invoke(args);

                // Gather Execute method arguments
                Log.Info($"Gathering properties for test block {tb}...");
                MethodInfo execute = tb.GetMethod("Execute");
                ParameterInfo[] argInfo = execute.GetParameters();
                args = ValidateAndFetchTestBlockParameters(argInfo);

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
                    ObjectBag.AddItemToBag(result);
                }
            }
        }

        private object[] ValidateAndFetchTestBlockParameters(ParameterInfo[] parameters)
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var t = parameters[i].ParameterType;
                if (!ObjectBag.TryGetItemFromBag(t, out object p))
                {
                    // Check for factories once that's implemented
                    string message = $"...expected an object of type {t.GetType()}, however none could be found in the item bag.";
                    Log.Info(message);
                    throw new ArgumentException(message);
                }
                args[i] = p;
            }
            Log.Info("Found all required items.");
            return args;
        }

        private List<Type> TestBlockTypes { get; set; } = new List<Type>();
        private TestObjectsBag ObjectBag { get; set; } = new TestObjectsBag();
        private Log Log { get; set; } = new Log();
    }
}
