using System;
using System.Globalization;

namespace IntelliTect.TestTools.TestFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseAttribute : Attribute
    {
        public TestCaseAttribute(string id = null, string name = null)
        {
            TestCaseId = id;
            TestCaseName = name;
        }

        public string TestCaseId { get; set; }
        public string TestCaseName { get; set; }
    }
}
