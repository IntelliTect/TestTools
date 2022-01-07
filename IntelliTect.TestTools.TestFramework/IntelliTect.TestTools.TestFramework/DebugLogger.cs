using System;
using System.Text.Json;

namespace IntelliTect.TestTools.TestFramework
{
    public class DebugLogger : ITestCaseLogger
    {
        public DebugLogger(TestCase testCase/*, IObjectSerializer serializer*/)
        {
            TestCase = testCase;
            //Serializer = serializer;
        }

        //public IObjectSerializer Serializer { get; set; }
        public TestCase TestCase { get; }
        //public string? TestCaseKey { get; set; }
        public string? CurrentTestBlock { get; set; }

        public void Debug(string message)
        {
            LogToDebug($"{TestCase.TestMethodName} - {CurrentTestBlock} - Debug: {message}");
        }

        public void Critical(string message)
        {
            LogToDebug($"{TestCase.TestMethodName} - {CurrentTestBlock} - Error: {message}");
        }

        public void Info(string message)
        {
            LogToDebug($"{TestCase.TestMethodName} - {CurrentTestBlock} - Info: {message}");
        }

        public void TestBlockInput(object input)
        {
            string inputString = Serialize(input);
            LogToDebug($"{TestCase.TestMethodName} - {CurrentTestBlock} - Input arguments: {inputString}");
        }

        public void TestBlockOutput(object output)
        {
            string outputString = Serialize(output);
            LogToDebug($"{TestCase.TestMethodName} - {CurrentTestBlock} - Output returns: {outputString}");
        }

        private void LogToDebug(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        // Is it worth making this injectable?
        // That way we can override just this...
        // However, we've only ever had to swap out loggers, not the actual serializer.
        private string Serialize(object objectToParse)
        {
            // JsonSerializer.Serialize has some different throw behavior between versions.
            // One version threw an exception that occurred on a property, which happened to be a Selenium WebDriverException.
            // In this one specific case, catch all exceptions and move on to provide standard behavior to all package consumers.
            // TL;DR: we don't want logging failures to interrupt the test run.
            try
            {
                return System.Text.Json.JsonSerializer.Serialize(objectToParse, new JsonSerializerOptions { WriteIndented = true });
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return $"Unable to serialize object {objectToParse?.GetType()} to JSON. Mark the relevant property with the [JsonIgnore] attribute: {e}";
            }
        }
    }
}
