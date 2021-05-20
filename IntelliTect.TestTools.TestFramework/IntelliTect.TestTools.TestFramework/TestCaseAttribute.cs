using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace IntelliTect.TestTools.TestFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseAttribute : Attribute
    {
        public TestCaseAttribute(string id = "", [CallerMemberName]string name = "")
        {
            TestCaseName = name;
            TestCaseId = id;
        }

        public string TestCaseId { get; set; }
        public string TestCaseName { get; set; }
    }
}
