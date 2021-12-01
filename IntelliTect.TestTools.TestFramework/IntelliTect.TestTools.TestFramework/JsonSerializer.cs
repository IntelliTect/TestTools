using System;
using System.Text.Json;

namespace IntelliTect.TestTools.TestFramework
{
    public class JsonSerializer : IObjectSerializer
    {
        public string Serialize(object objectToParse)
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
