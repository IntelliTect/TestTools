﻿using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestBuilder
    {

        public TestBuilder([CallerMemberName] string? testMethodName = null)
        {
            TestMethodName = testMethodName ?? "UndefinedTestMethodName";
            AddLogger<DebugLogger>();
        }

        /// <summary>
        /// Used when a test case may be associated to a unique ID
        /// </summary>
        /// <param name="testCaseKey"></param>
        /// <returns></returns>
        public TestBuilder AddTestCaseId(int testCaseKey)
        {
            TestCaseId = testCaseKey;
            return this;
        }

        /// <summary>
        /// Used to give a friendly name to the test case
        /// </summary>
        /// <param name="testCaseName"></param>
        /// <returns></returns>
        public TestBuilder AddTestCaseName(string testCaseName)
        {
            TestCaseName = testCaseName;
            return this;
        }

        private List<(Type TestBlockType, object[]? TestBlockParameters)> TestBlocksAndParams { get; } = new List<(Type TestBlockType, object[]? TestBlockParameters)>();
        private List<(Type TestBlockType, object[]? TestBlockParameters)> FinallyBlocksAndParams { get; } = new List<(Type TestBlockType, object[]? TestBlockParameters)>();
        private IServiceCollection Services { get; } = new ServiceCollection();
        private HashSet<object> TestBlockResults { get; } = new HashSet<object>();
        private int TestCaseId { get; set; }
        private string TestCaseName { get; set; }
        private string TestMethodName { get; set; }
        private Exception? TestBlockException { get; set; }

        /// <summary>
        /// Adds a test block (some related group of test actions) to the list of blocks to run for any given test case
        /// </summary>
        /// <typeparam name="T">The type of test block, as an ITestBlock, to run</typeparam>
        /// <returns>This</returns>
        public TestBuilder AddTestBlock<T>() where T : ITestBlock
        {
            TestBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: null));
            Services.AddTransient(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a test block (some related group of test actions) with a list of arguments 
        /// that must match the associated TestBlock.Execute() method to the list of blocks to run for any given test case
        /// </summary>
        /// <typeparam name="T">The type of dependency a test block needs to execute</typeparam>
        /// <param name="testBlockArgs">The list of arguments to fulfill a set of Execute(params object[]) parameters</param>
        /// <returns>This</returns>
        public TestBuilder AddTestBlock<T>(params object[] testBlockArgs) where T : ITestBlock
        {
            // Instead of keeping track of test blocks and parameters, would it make sense to add the parameter to the service provider?
            // That would break the explicit link, but would simplify the logic.
            // Consider and re-visit.
            TestBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: testBlockArgs));
            Services.AddTransient(typeof(T));
            return this;
        }

        public TestBuilder AddFinallyBlock<T>(params object[] testBlockArgs) where T : ITestBlock
        {
            FinallyBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: testBlockArgs));
            Services.AddTransient(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a service as a factory a container that is used to fulfill TestBlock dependencies
        /// </summary>
        /// <typeparam name="T">The type of dependency a test block needs to execute</typeparam>
        /// <param name="serviceFactory">The factory to provide an instance of the type needed for a test block to execute</param>
        /// <returns></returns>
        public TestBuilder AddDependencyService<T>(Func<IServiceProvider, object> serviceFactory)
        {
            Services.AddScoped(typeof(T), serviceFactory);
            return this;
        }

        /// <summary>
        /// Adds a service as a Type to the container that is used to fulfill TestBlock dependencies
        /// </summary>
        /// <typeparam name="T">The type of test block, as an ITestBlock, to run</typeparam>
        /// <returns>This</returns>
        public TestBuilder AddDependencyService<T>()
        {
            Services.AddScoped(typeof(T));
            return this;
        }

        public TestBuilder AddDependencyService<TServiceType, TImplementationType>()
        {
            Services.AddScoped(typeof(TServiceType), typeof(TImplementationType));
            return this;
        }

        /// <summary>
        /// Adds an instance of a Type to the container that is needed for a TestBlock to execute
        /// </summary>
        /// <param name="objToAdd">The instance of a Type that a TestBlock needs</param>
        /// <returns>This</returns>
        public TestBuilder AddDependencyInstance(object objToAdd)
        {
            if (objToAdd is null) throw new ArgumentNullException(nameof(objToAdd));
            // Need to add some testing around this to see if it behaves in a similarly odd fashion as AddLogger when running tests in parallel
            Services.AddSingleton(objToAdd.GetType(), objToAdd);
            return this;
        }

        public TestBuilder AddDependencyInstance<T>(object objToAdd)
        {
            Services.AddSingleton(typeof(T), objToAdd);
            return this;
        }

        // Are there other cases where we'll need to add something at this level?
        // If so, this shouldn't be called "AddLogger".
        // Might need to make this scoped. It's behaving oddly when running tests in parallel
        // But only on the "Starting test case" call
        public TestBuilder AddLogger<T>() where T : ILogger
        {
            RemoveLogger();
            Services.AddSingleton(typeof(ILogger), typeof(T));
            return this;
        }

        public TestBuilder RemoveLogger()
        {
            var logger = Services.FirstOrDefault(d => d.ServiceType == typeof(ILogger));
            Services.Remove(logger);
            return this;
        }

        public TestCase Build()
        {
            ServiceProvider? provider = Services.BuildServiceProvider();

            TestCase testCase = new()
            {
                TestMethodName = TestMethodName,
                TestCaseName = TestCaseName,
                TestCaseId = TestCaseId,
                Services = provider
            };

            List<string> validationMessages = new();
            // Probably need to profile all of this for performance at some point.
            // Need to make sure if we're running hundreds or thousands of tests that we're not adding significant amount of time to that.
            foreach(var tb in TestBlocksAndParams)
            {
                // Step through this. How do we handle constructors?
                // May need to explicitly call constructors out in case it's being handled by MS DI magic.
                Type type = tb.TestBlockType;
                ConstructorInfo[]? constructors = type.GetConstructors();
                
                if(constructors.Length > 1)
                {
                    throw new InvalidOperationException(
                        $"TestBlock: {type} - Currently TestFramework supports zero or one constructors on test blocks.");
                }

                ParameterInfo[]? ctorParams = constructors[0].GetParameters();
                PropertyInfo[]? properties = type.GetProperties();
                List<MethodInfo>? methods = type.GetMethods().Where(m => m.Name.ToUpperInvariant() == "EXECUTE").ToList();
                if (methods.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"TestBlock: {type} - There must be one and only one Execute method on a test block.");
                }
                ParameterInfo[]? executeParams = methods[0].GetParameters();

                List<Type> inputs = new();
                foreach(var c in ctorParams)
                { 
                    inputs.Add(c.ParameterType);
                }
                foreach(var p in properties)
                {
                    inputs.Add(p.PropertyType);
                }
                foreach(var e in executeParams)
                {
                    inputs.Add(e.ParameterType);
                }

                Type executeReturns = methods[0].ReturnType;
                List<Type> outputs = new();
                if (tb.TestBlockParameters is not null)
                {
                    foreach(var p in tb.TestBlockParameters)
                    {
                        outputs.Add(p.GetType());
                    }
                }
                if (executeReturns != typeof(void))
                {
                    outputs.Add(executeReturns);
                }

                foreach (var i in inputs)
                {
                    object? blockInput = provider.GetService(i);
                    if (blockInput is null)
                    {
                        blockInput = outputs.FirstOrDefault(o => o == i);
                    }
                    if (blockInput is null)
                    {
                        validationMessages.Add($"TestBlock: {type} - Unable to satisfy input: {i}");
                    }
                }
            }

            if(validationMessages.Count > 0)
            {
                // This might make sense as an AggregateException instead
                string message = "";
                foreach(var vm in validationMessages)
                {
                    message += "\r\n";
                    message += vm;
                }
                throw new InvalidOperationException(message);
            }
            // add test blocks to test case
            // add finally blocks to test case
            // validate inputs and outputs (maybe do that first?)

            Services.AddSingleton(testCase);
            return testCase;

            //void AddType(object[] objectToAdd, IList<object> listToAdd)
            //{
            //    foreach(var o in objectToAdd)
            //    {
            //        listToAdd.Add(o.GetType());
            //    }
            //}
        }

        // Legacy support
        //public void ExecuteTestCase()
        //{
        //    TestCase tc = Build();
        //    tc.ExecuteTestCase();
        //}

        public void ExecuteTestCase()
        {
            #region move to a Build() method and validate all dependencies are satisfied?
            var serviceProvider = Services.BuildServiceProvider();
            #endregion

            using (var testCaseScope = serviceProvider.CreateScope())
            {
                var logger = testCaseScope.ServiceProvider.GetService<ILogger>();
                if (logger != null)
                {
                    logger.TestCaseKey = TestCaseName;
                    logger.CurrentTestBlock = "N/A";
                }

                logger?.Info("Starting test case.");

                using (var testBlockScope = serviceProvider.CreateScope())
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

            serviceProvider.Dispose();

            if (TestBlockException != null)
            {
                throw new TestCaseException("Test case failed.", TestBlockException);
            }
        }

        private static string GetObjectDataAsJsonString(object obj)
        {
            // JsonSerializer.Serialize has some different throw behavior between versions.
            // One version threw an exception that occurred on a property, which happened to be a Selenium WebDriverException.
            // In this one specific case, catch all exceptions and move on to provide standard behavior to all package consumers.
            // TL;DR: we don't want logging failures to interrupt the test run.
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                return $"Unable to serialize object {obj?.GetType()} to JSON. Mark the relevant property with the [JsonIgnore] attribute: {e}";
            }

        }

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
                logger?.TestBlockInput(GetObjectDataAsJsonString(arg));
            }

            try
            {
                var result = execute.Invoke(testBlockInstance, executeArgs);
                if (result != null)
                {
                    logger?.TestBlockOutput(GetObjectDataAsJsonString(result));
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
