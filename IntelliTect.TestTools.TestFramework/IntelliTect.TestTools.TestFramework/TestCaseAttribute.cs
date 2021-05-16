using System;
using System.Globalization;

namespace IntelliTect.TestTools.TestFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseAttribute : Attribute
    {
        public TestCaseAttribute(string name = null, string id = null)
        {
            TestCaseName = name;
            TestCaseId = id;
        }

        public string TestCaseId { get; set; }
        public string TestCaseName { get; set; }
    }
}
