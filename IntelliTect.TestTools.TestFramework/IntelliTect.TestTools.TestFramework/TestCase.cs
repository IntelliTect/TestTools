using System;
using System.Collections.Generic;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestCase
    {
        public string TestCaseName { get; set; }
        public string TestMethodName { get; set; }
        private List<(Type TestBlockType, object[] TestBlockParameters)> TestBlocksAndParams { get; } = new();
        private List<(Type TestBlockType, object[] TestBlockParameters)> FinallyBlocksAndParams { get; } = new();
        private IServiceProvider Services { get; }
        // Inputs
        // Outputs
        // TestCaseAttributes
        public bool Passed { get; set; }

        //public void ExecuteTestCase()
        //{

        //}
    }
}
