﻿using System;
using System.Collections.Generic;
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
        internal bool IsFinallyBlock { get; set; }
        // Test if this is faster than arrays.
        //HashSet<ParameterInfo> ConstructorParams = new HashSet<ParameterInfo>(block.ConstructorParams);
        // May not need this since activating the test block from DI will auto-populate these.
        internal ParameterInfo[] ConstructorParams { get; set; } = Array.Empty<ParameterInfo>();
        internal MethodInfo ExecuteMethod { get; set; }
        internal ParameterInfo[] ExecuteParams { get; set; } = Array.Empty<ParameterInfo>();
        internal Dictionary<Type, object> ExecuteArgumentOverrides { get; set; } = new();
        internal PropertyInfo[] PropertyParams { get; set; } = Array.Empty<PropertyInfo>();
        // Is this needed?
        //internal ParameterInfo[]? FieldParams { get; set; }
    }
}