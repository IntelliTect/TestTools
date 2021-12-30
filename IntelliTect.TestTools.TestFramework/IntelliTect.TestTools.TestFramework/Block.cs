using System;
using System.Reflection;

namespace IntelliTect.TestTools.TestFramework
{
    internal class Block
    {
        public Block(Type type, MethodInfo execute)
        {
            Type = type;
            ExecuteMethod = execute;
        }

        internal Type Type { get; set; }
        // May not need this since activating the test block from DI will auto-populate these.
        //internal ParameterInfo[]? ConstructorParams { get; set; }
        internal MethodInfo ExecuteMethod { get; set; }
        internal ParameterInfo[]? ExecuteParams { get; set; }
        internal PropertyInfo[]? PropertyParams { get; set; }
        internal object[]? ExecuteArgumentOverrides { get; set; }
        // Is this needed?
        //internal ParameterInfo[]? FieldParams { get; set; }
    }
}
