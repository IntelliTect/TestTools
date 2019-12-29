using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
        public TestBuilder([CallerMemberName]string testCaseKey = null)
        {
            TestCaseName = testCaseKey;
            AddLogger<Log>();
        }

        public TestBuilder OverrideTestCaseKey([CallerMemberName]string testCaseKey = null)
        {
            TestCaseName = testCaseKey;
            return this;
        }

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

        /// <summary>
        /// Adds an instance of a Type to the container that is needed for a TestBlock to execute
        /// </summary>
        /// <param name="objToAdd">The instance of a Type that a TestBlock needs</param>
        /// <returns>This</returns>
        public TestBuilder AddDependencyInstance(object objToAdd)
        {
            // Need to add some testing around this to see if it behaves in a similarly odd fashion as AddLogger when running tests in parallel
            Services.AddSingleton(objToAdd.GetType(), objToAdd);
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

        public void ExecuteTestCase()
        {
            #region move to a Build() method and validate all dependencies are satisfied?
            var serviceProvider = Services.BuildServiceProvider();
            #endregion


            //Exception testBlockException = null;
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
                    HashSet<object> testBlockResults = new HashSet<object>();
                    foreach (var tb in TestBlocksAndParams)
                    {
                        if (logger != null) logger.CurrentTestBlock = tb.TestBlockType.ToString();
                        // Might be more concise to have these as out method parameters instead of if statements after every one
                        var testBlockInstance = GetTestBlock(testBlockScope, tb.TestBlockType, logger);
                        if (TestBlockException != null) break;

                        SetTestBlockProperties(testBlockScope, testBlockInstance, logger);
                        if (TestBlockException != null) break;

                        MethodInfo execute = GetExecuteMethod(testBlockScope, testBlockInstance);
                        if (TestBlockException != null) break;

                        var executeArgs = GatherTestBlockArguments(testBlockScope, execute, tb, logger);
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
                        var testBlockInstance = GetTestBlock(testBlockScope, fb.TestBlockType, logger);
                        if (TestBlockException != null) break;

                        SetTestBlockProperties(testBlockScope, testBlockInstance, logger);
                        if (TestBlockException != null) break;

                        MethodInfo execute = GetExecuteMethod(testBlockScope, testBlockInstance);
                        if (TestBlockException != null) break;

                        var executeArgs = GatherTestBlockArguments(testBlockScope, execute, fb, logger);
                        if (TestBlockException != null) break;

                        RunTestBlocks(testBlockInstance, execute, executeArgs, logger);
                        if (TestBlockException != null) break;
                    }
                    TestBlockException = tempException;
                }
            }

            serviceProvider.Dispose();

            if (TestBlockException != null)
            {
                throw new TestCaseException("Test case failed.", TestBlockException);
            }

        }

        private string GetObjectDataAsJsonString(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            }
            catch (JsonSerializationException e)
            {
                return $"Unable to serialize to JSON. Mark the relevant property or constructor with the [JsonIgnore] attribute: {e.Message}";
            }
        }

        private object GetTestBlock(IServiceScope scope, Type tbType, ILogger logger)
        {
            logger?.Info("Starting test block.");

            try
            {
                return scope.ServiceProvider.GetRequiredService(tbType);
            }
            catch (InvalidOperationException ex)
            {
                // Probably worth moving these logs outside of the foreach so we don't have to duplicate the message
                logger?.Error("Unable to find the test block instance OR all dependencies necessary.");
                TestBlockException = ex;
                return null;
            }

        }

        private void SetTestBlockProperties(IServiceScope scope, object testBlockInstance, ILogger logger)
        {
            // Populate and log all of our properties
            var properties = testBlockInstance.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (!prop.CanWrite)
                {
                    logger?.Debug($"Skipping property {prop}. No setter found.");
                    continue;
                }
                object propertyValue;
                try
                {
                    propertyValue = scope.ServiceProvider.GetRequiredService(prop.PropertyType);
                }
                catch (InvalidOperationException ex)
                {
                    logger?.Error($"Unable to find all properties necessary.");
                    TestBlockException = ex;
                    break;
                }

                prop.SetValue(testBlockInstance, propertyValue);
                logger?.Debug($"Populated property {prop.Name} with data: {GetObjectDataAsJsonString(prop.GetValue(testBlockInstance))}");
            }
        }

        private MethodInfo GetExecuteMethod(IServiceScope scope, object testBlockInstance)
        {
            List<MethodInfo> methods = testBlockInstance.GetType().GetMethods().Where(m => m.Name.ToUpperInvariant() == "EXECUTE").ToList();
            if (methods.Count != 1)
            {
                TestBlockException = new InvalidOperationException($"There can be one and only one Execute method on a test block. " +
                    $"Please review test block {testBlockInstance.GetType().ToString()}.");
                return null;
            }

            return methods[0];
        }

        private object[] GatherTestBlockArguments(IServiceScope scope, MethodInfo execute, (Type TestBlockType, object[] TestBlockParameters) tb, ILogger logger)
        {
            var executeParams = execute.GetParameters();

            object[] executeArgs = new object[executeParams.Length];

            // Is this the right order of checking? Or should we prioritize test block results first?
            // Initial thought is that if someone is passing in explicit arguments, they probably have a good reason, so we should start there
            // Populate and log all of our Execute arguments
            if (executeArgs.Length > 0)
            {
                if (tb.TestBlockParameters != null && executeParams.Length == tb.TestBlockParameters.Length)
                {
                    // Eventually need to add more validation around making sure the types match here.
                    executeArgs = tb.TestBlockParameters;
                }
                else
                {
                    for (int i = 0; i < executeArgs.Length; i++)
                    {
                        // Might be worth changing this to GetRequiredService and wrapping in a try/catch instead of checking if foundResult is null
                        object foundResult =
                            TestBlockResults.FirstOrDefault(tbr => tbr.GetType() == executeParams[i].ParameterType)
                            ?? scope.ServiceProvider.GetService(executeParams[i].ParameterType);
                        if (foundResult == null)
                        {
                            TestBlockException = new InvalidOperationException("Unable to resolve Execute method arguments");
                            break;
                        }
                        executeArgs[i] = foundResult;
                    }
                }

                // Instead of doing this, might be worth extracting the above for loop into a private method and if that fails, then break out of the foreach we're in now
                if (TestBlockException != null)
                    return null;

                foreach (var arg in executeArgs)
                {
                    logger?.TestBlockInput($"Input parameters: {GetObjectDataAsJsonString(arg)}");
                }
            }
            return executeArgs;
        }

        private void RunTestBlocks(object testBlockInstance, MethodInfo execute, object[] executeArgs, ILogger logger)
        {
            try
            {
                logger?.Debug($"Executing test block");
                var result = execute.Invoke(testBlockInstance, executeArgs);
                if (result != null)
                {
                    logger?.TestBlockOutput($"Output parameters: {GetObjectDataAsJsonString(result)}");
                    TestBlockResults.Add(result);
                }

            }
            catch (TargetInvocationException ex)
            {
                logger?.Error($"---Test block failed.---");
                TestBlockException = ex.InnerException;
                return;
            }
            catch (ArgumentException ex)
            {
                logger?.Error($"---Test block failed.---");
                TestBlockException = ex;
                return;
            }
            catch (TargetParameterCountException ex)
            {
                logger?.Error($"---Mismatched count between Execute method arguments and supplied dependencies. Test block failed.---");
                TestBlockException = ex;
                return;
            }

            logger?.Info($"---Test block completed successfully.---");
        }

        private List<(Type TestBlockType, object[] TestBlockParameters)> TestBlocksAndParams { get; } = new List<(Type TestBlockType, object[] TestBlockParameters)>();
        private List<(Type TestBlockType, object[] TestBlockParameters)> FinallyBlocksAndParams { get; } = new List<(Type TestBlockType, object[] TestBlockParameters)>();
        private IServiceCollection Services { get; } = new ServiceCollection();
        private HashSet<object> TestBlockResults { get; } = new HashSet<object>();
        private string TestCaseName { get; set; }
        private Exception TestBlockException { get; set; }
    }
}
