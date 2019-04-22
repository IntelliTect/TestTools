using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public TestBuilder AddTestBlock(MethodInfo method)
        {
            method.Invoke(method.DeclaringType, null);
            return this;
        }

        public void ExecuteTest()
        {
            //foreach(var testBlock in TestBlocks)
            //{
            //    testBlock();
            //    Debug.WriteLine(testBlock.Method);
            //}
            foreach(var tb in TestBlockTypes)
            {
                var parameters = tb..GetParameters();
                foreach(var p in parameters)
                {

                }
            }
        }

        private List<Type> TestBlockTypes { get; set; } = new List<Type>();
        private List<Action> TestBlocks { get; set; } = new List<Action>();
        private List<object> Data { get; set; } = new List<object>();
    }
}
