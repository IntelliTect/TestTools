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
        //    TestBlocks2.Add(method);
        //    return this;
        //}

        //public TestBuilder AddTestBlock(MethodInfo method)
        //{
        //    TestBlocks2.Add(method);
        //    return this;
        //}

        //public TestBuilder AddTestBlock<T, TResult>(Func<T, TResult> method)
        //{
        //    TestBlocks2.Add(method);
        //    return this;
        //}

        //public TestBuilder AddTestBlock<T1, T2, TResult>(Func<T1, T2, TResult> method)
        //{
        //    TestBlocks2.Add(method);
        //    return this;
        //}

        //public TestBuilder AddTestBlock<TResult>(params Func<TResult>[] methods)
        //{
        //    TestBlocks2.AddRange(methods);
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

        //public void ExecuteTestByDelegate()
        //{
        //    //foreach (Action tb in TestBlocks)
        //    //{
        //    //    Debug.WriteLine($"Starting test block {tb.Method.Name}");
        //    //    Stopwatch sw = new Stopwatch();
        //    //    var test = tb.Method.GetParameters();
        //    //    sw.Start();
        //    //    tb();
        //    //    sw.Stop();
        //    //    Debug.WriteLine($"Time for test block to execute: {sw.Elapsed}");
        //    //}
        //    foreach (Delegate tb in TestBlocks2)
        //    {
        //        Debug.WriteLine($"Starting test block {tb.Method.Name}");
        //        Stopwatch sw = new Stopwatch();
        //        var test = tb.Method.GetParameters();
        //        sw.Start();
        //        object result = tb.Method.Invoke(tb.Target, new object[] { });
        //        sw.Stop();
        //        Debug.WriteLine($"Time for test block to execute: {sw.Elapsed}");
        //    }
        //}

        public void ExecuteTestBlock()
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
        private List<Action> TestBlocks { get; set; } = new List<Action>();
        private List<Delegate> TestBlocks2 { get; set; } = new List<Delegate>();
        private List<object> Data { get; set; } = new List<object>();
    }
}
