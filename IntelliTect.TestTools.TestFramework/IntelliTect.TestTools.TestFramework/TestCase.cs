using System;
using System.Collections.Generic;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestCase
    {
        public string TestCaseName { get; set; /*init;*/ } // Investigate and fix issue with init;
        public string TestMethodName { get; set; }
        public int TestCaseId { get; set; }
        public IServiceProvider Services { get; set; }
        private List<(Type TestBlockType, object[] TestBlockParameters)> TestBlocksAndParams { get; } = new();
        private List<(Type TestBlockType, object[] TestBlockParameters)> FinallyBlocksAndParams { get; } = new();

        // Inputs
        // Outputs
        
        public bool Passed { get; set; }

        public static void ExecuteTestCase()
        {
            //string testing = null;
            //TestCaseName = testMethodName;
        }
    }
}
