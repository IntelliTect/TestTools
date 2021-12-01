using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestCase
    {
        // Switch to get; init; when this can be updated to .net5
        // Maybe target .net5 support for v3?
        public string TestCaseName { get; set; } 
        public string TestMethodName { get; set; }
        public int TestCaseId { get; set; }
        public ServiceProvider Services { get; set; }
        public bool ThrowOnFinallyBlockException { get; set; } = true;
        private List<(Type TestBlockType, object[] TestBlockParameters)> TestBlocksAndParams { get; } = new();
        private List<(Type TestBlockType, object[] TestBlockParameters)> FinallyBlocksAndParams { get; } = new();
        // Inputs?
        // Outputs?

        // TEMPORARY TO TEST
        private Exception? TestBlockException { get; set; }

        // UNSURE IF NEEDED WITH NEW BUILD METHOD
        private List<object> TestBlockResults { get; } = new();
        //private IObjectSerializer? Serializer { get; set; }

        public bool Passed { get; set; }

        public void ExecuteTestCase()
        {
            using (var testCaseScope = Services.CreateScope())
            {
                ILogger? logger = testCaseScope.ServiceProvider.GetService<ILogger>();
                if (logger != null)
                {
                    logger.TestCaseKey = TestCaseName;
                    logger.CurrentTestBlock = "N/A";
                    logger.Serializer = testCaseScope.ServiceProvider.GetService<IObjectSerializer>();
                }

                logger?.Info($"Starting test case: {TestCaseName}");

                using (var testBlockScope = Services.CreateScope())
                {
                    foreach (var tb in TestBlocksAndParams)
                    {
                        if (logger != null) logger.CurrentTestBlock = tb.TestBlockType.ToString();
                        // Might be more concise to have these as out method parameters instead of if statements after every one
                        var testBlockInstance = GetTestBlock(testBlockScope, tb.TestBlockType);
                        if (TestBlockException != null) break;

                        SetTestBlockProperties(testBlockScope, testBlockInstance, logger);
                        if (TestBlockException != null) break;

                        MethodInfo execute = GetExecuteMethod(testBlockInstance);
                        if (TestBlockException != null) break;

                        var executeArgs = GatherTestBlockArguments(testBlockScope, execute, tb);
                        if (TestBlockException != null) break;

                        RunTestBlocks(testBlockInstance, execute, executeArgs, logger);
                        if (TestBlockException != null) break;
                    }

                    // Need a much better way to handle Finally exceptions...
                    Exception tempException = TestBlockException;
                    TestBlockException = null;
                    // Extract loop above since it's basically the same for finally blocks?
                    foreach (var fb in FinallyBlocksAndParams)
                    {
                        if (logger != null) logger.CurrentTestBlock = fb.TestBlockType.ToString();
                        // Might be more concise to have these as out method parameters instead of if statements after every one
                        // Also these specific ones should not be overwriting TestBlockException
                        var testBlockInstance = GetTestBlock(testBlockScope, fb.TestBlockType);
                        if (TestBlockException != null) break;

                        SetTestBlockProperties(testBlockScope, testBlockInstance, logger);
                        if (TestBlockException != null) break;

                        MethodInfo execute = GetExecuteMethod(testBlockInstance);
                        if (TestBlockException != null) break;

                        var executeArgs = GatherTestBlockArguments(testBlockScope, execute, fb);
                        if (TestBlockException != null) break;

                        RunTestBlocks(testBlockInstance, execute, executeArgs, logger);
                        if (TestBlockException != null) break;
                    }
                    TestBlockException = tempException;
                }

                if (TestBlockException == null)
                {
                    logger?.Info("Test case finished successfully.");
                }
                else
                {
                    logger?.Critical($"Test case failed: {TestBlockException}");
                }
            }

            Services.Dispose();

            if (TestBlockException != null)
            {
                throw new TestCaseException("Test case failed.", TestBlockException);
            }
        }

        // TEMPORARY
        // RE-WRITE ALL OF THIS NOW THAT WE BUILD FIRST
        private object GetTestBlock(IServiceScope scope, Type tbType)
        {
            var tb = scope.ServiceProvider.GetService(tbType);
            if (tb == null)
            {
                TestBlockException = new InvalidOperationException($"Unable to find test block: {tbType.FullName}.");
            }

            return tb;
        }

        private void SetTestBlockProperties(IServiceScope scope, object testBlockInstance, ILogger logger)
        {
            // Populate all of our properties
            var properties = testBlockInstance.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (!prop.CanWrite)
                {
                    logger?.Debug($"Skipping property {prop}. No setter found.");
                    continue;
                }
                object propertyValue = scope.ServiceProvider.GetService(prop.PropertyType);
                if (propertyValue == null)
                {
                    TestBlockException = new InvalidOperationException($"Unable to find an object or service for property {prop.Name} of type {prop.PropertyType.FullName} on test block {testBlockInstance.GetType()}.");
                    break;
                }

                prop.SetValue(testBlockInstance, propertyValue);
            }
        }

        private MethodInfo GetExecuteMethod(object testBlockInstance)
        {
            List<MethodInfo> methods = testBlockInstance.GetType().GetMethods().Where(m => m.Name.ToUpperInvariant() == "EXECUTE").ToList();
            // No longer need to do this check since we now do it in Build()
            // UNLESS we end up supporting multiple Execute methods, then there may still be a need to double check at run-time?
            if (methods.Count != 1)
            {
                TestBlockException = new InvalidOperationException($"There can be one and only one Execute method on a test block. " +
                    $"Please review test block {testBlockInstance.GetType()}.");
                return null;
            }

            return methods[0];
        }

        private object[] GatherTestBlockArguments(IServiceScope scope, MethodInfo execute, (Type TestBlockType, object[] TestBlockParameters) tb)
        {
            var executeParams = execute.GetParameters();

            object[] executeArgs = new object[executeParams.Length];

            // Is this the right order of checking? Or should we prioritize test block results first?
            // Initial thought is that if someone is passing in explicit arguments, they probably have a good reason, so we should start there
            // Populate and log all of our Execute arguments
            if (executeArgs.Length > 0)
            {
                // We should change this to just match on type instead of length of args
                // That way you can override just a single parameter if desired. That seems to be the primary (albeit rare) use case here.
                if (tb.TestBlockParameters != null && executeParams.Length == tb.TestBlockParameters.Length)
                {
                    // Eventually need to add more validation around making sure the types match here.
                    executeArgs = tb.TestBlockParameters;
                }
                else
                {
                    for (int i = 0; i < executeArgs.Length; i++)
                    {
                        object foundResult = TestBlockResults.FirstOrDefault(tbr => tbr.GetType() == executeParams[i].ParameterType)
                            ?? scope.ServiceProvider.GetService(executeParams[i].ParameterType);
                        if (foundResult == null)
                        {
                            TestBlockException = new InvalidOperationException($"Unable to find an object or service for Execute parameter {executeParams[i].Name} of type {executeParams[i].ParameterType.FullName} on test block {tb.TestBlockType.FullName}.");
                            break;
                        }

                        executeArgs[i] = foundResult;
                    }
                }

                // Instead of doing this, might be worth extracting the above for loop into a private method and if that fails, then break out of the foreach we're in now
                if (TestBlockException != null) return null;
            }
            return executeArgs;
        }

        private void RunTestBlocks(object testBlockInstance, MethodInfo execute, object[] executeArgs, ILogger logger)
        {
            logger?.Debug($"Starting test block.");
            // Log ALL inputs
            // Is it worth distinguishing between Properties and Execute args?
            PropertyInfo[] props = testBlockInstance.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
            object[] allArgs = new object[props.Length + executeArgs.Length];

            for (int i = 0; i < props.Length; i++)
            {
                allArgs[i] = props[i].GetValue(testBlockInstance);
            }

            executeArgs.CopyTo(allArgs, props.Length);
            foreach (var arg in allArgs)
            {
                logger?.TestBlockInput(arg);
            }

            try
            {
                var result = execute.Invoke(testBlockInstance, executeArgs);
                if (result != null)
                {
                    logger?.TestBlockOutput(result);
                    TestBlockResults.Add(result);
                }

            }
            catch (TargetInvocationException ex)
            {
                TestBlockException = ex.InnerException;
                return;
            }
            catch (ArgumentException ex)
            {
                TestBlockException = ex;
                return;
            }
            catch (TargetParameterCountException ex)
            {
                ex.Data.Add("AdditionalInfo", "Test block failed: Mismatched count between Execute method arguments and supplied dependencies.");
                TestBlockException = ex;
                return;
            }

            logger?.Debug($"Test block completed successfully.");
        }
    }
}
