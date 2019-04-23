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

        //public TestBuilder AddTestBlock(Action method)
        //{
        //    TestBlocks.Add(method);
        //    return this;
        //}

        //public TestBuilder AddTestBlock(params Action[] methods)
        //{
        //    TestBlocks.AddRange(methods);
        //    return this;
        //}

        public TestBuilder AddTestBlock<T>()
        {
            TestBlockTypes.Add(typeof(T));
            return this;
        }

        public TestBuilder AddData<T>()
        {
            Data.Add(default(T));
            return this;
        }

        public TestBuilder AddData(object data)
        {
            Data.Add(data);
            return this;
        }

        public TestBuilder AddData(params object[] data)
        {
            Data.AddRange(data);
            return this;
        }

        public TestBuilder AddTestBlock(MethodInfo method)
        {
            method.Invoke(method.DeclaringType, null);
            return this;
        }

        public void ExecuteTest()
        {
            foreach (Type tb in TestBlockTypes)
            {
                MethodInfo execute = tb.GetMethod("Execute");
                ParameterInfo[] argInfo = execute.GetParameters();

                object[] args = new object[argInfo.Length];
                for (int i = 0; i < argInfo.Length; i++)
                {
                    var t = argInfo[i].ParameterType;
                    var p = Data.Single(d => d.GetType() == t);
                    args[i] = p;
                }

                ConstructorInfo ctor = tb.GetConstructor(Type.EmptyTypes);
                object tbInstance = ctor.Invoke(new object[] {});


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
                //var parameters = tb..GetParameters();
                ////foreach (var p in parameters)
                //{

                //}
            }
        }

        private List<Type> TestBlockTypes { get; set; } = new List<Type>();
        //private List<Action> TestBlocks { get; set; } = new List<Action>();
        private List<object> Data { get; set; } = new List<object>();
    }
}
