using IntelliTect.TestTools.TestFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleTests
{
    class NewLogger : ILogger
    {
        public void Debug(string testCase, string testBlock, string message)
        {
            Log($"{testCase} - {testBlock} - Debug: {message}");
        }

        public void Error(string testCase, string testBlock, string message)
        {
            Log($"{testCase} - {testBlock} - Error: {message}");
        }

        public void Info(string testCase, string testBlock, string message)
        {
            Log($"{testCase} - {testBlock} - Info: {message}");
        }

        public void TestBlockInput(string testCase, string testBlock, string input)
        {
            Log($"{testCase} - {testBlock} - Input: {input}");
        }

        public void TestBlockOutput(string testCase, string testBlock, string output)
        {
            Log($"{testCase} - {testBlock} - Output: {output}");
        }

        private void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Console.WriteLine(message);
        }
    }
}
