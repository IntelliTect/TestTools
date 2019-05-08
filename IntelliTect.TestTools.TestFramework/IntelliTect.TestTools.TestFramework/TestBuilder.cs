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

        // Should this return a new type?
        public TestBuilder Build()
        {
            foreach (Type tb in TestBlockTypes)
            {
                ConstructorInfo[] ctors = tb.GetConstructors();
                ParameterInfo[] ctorArgs = ctors.First().GetParameters();
                object[] args = GetArguments(ctorArgs);

                // Create instance of test block
                object tbInstance = ctors.First().Invoke(args);

                // Gather properties
                MethodInfo execute = tb.GetMethod("Execute");
                ParameterInfo[] argInfo = execute.GetParameters();
                args = GetArguments(argInfo);
            }
            return this;
        }

        public void ExecuteTestBlock()
        {
            // Check for non-existent types here

            // Execute test blocks
            foreach (Type tb in TestBlockTypes)
            {
                // Get constructor
                ConstructorInfo[] ctors = tb.GetConstructors();
                ParameterInfo[] ctorArgs = ctors.First().GetParameters();
                object[] args = GetArguments(ctorArgs);

                // Create instance of test block
                object tbInstance = ctors.First().Invoke(args);

                // Gather properties
                MethodInfo execute = tb.GetMethod("Execute");
                ParameterInfo[] argInfo = execute.GetParameters();
                args = GetArguments(argInfo);

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

        private object[] GetArguments(ParameterInfo[] parameters)
        {
            object[] args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var t = parameters[i].ParameterType;
                if (!TryGetItemFromBag(t, out object p))
                    throw new Exception("Could not find type in bag. Eventually replace this with a new exception");
                args[i] = p;
            }
            return args;
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

        private bool TryGetItemFromBag(Type typeToFind, out object data)
        {
            data = Data.SingleOrDefault(d => d.GetType() == typeToFind);
            if (data == null)
            {
                data = Data.SingleOrDefault(d => d.GetType().BaseType == typeToFind);
                if (data == null)
                {
                    // This will produce unexpected results if we load up two different browser types. It will grab whatever is first.
                    data = Data.Single(d => d.GetType().GetInterfaces().ToList().Contains(typeToFind));
                }
            }

            return data != null ? true : false;
        }

        private List<Type> TestBlockTypes { get; set; } = new List<Type>();
        private List<object> Data { get; set; } = new List<object>();
        private Log Log { get; set; } = new Log();
    }
}
