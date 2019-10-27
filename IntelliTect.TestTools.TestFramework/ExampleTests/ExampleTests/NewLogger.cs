using IntelliTect.TestTools.TestFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleTests
{
    class NewLogger : ILogger
    {
        public string TestCaseKey { get; set; }
        public string CurrentTestBlock { get; set; }

        public void Debug(string message)
        {
            Log($"{TestCaseKey} - {CurrentTestBlock} - Debug: {message}");
        }

        public void Error(string message)
        {
            Log($"{TestCaseKey} - {CurrentTestBlock} - Error: {message}");
        }

        public void Info(string message)
        {
            Log($"{TestCaseKey} - {CurrentTestBlock} - Info: {message}");
        }

        public void TestBlockInput(string input)
        {
            Log($"{TestCaseKey} - {CurrentTestBlock} - Input: {input}");
        }

        public void TestBlockOutput(string output)
        {
            Log($"{TestCaseKey} - {CurrentTestBlock} - Output: {output}");
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Console.WriteLine(message);
        }
    }
}
