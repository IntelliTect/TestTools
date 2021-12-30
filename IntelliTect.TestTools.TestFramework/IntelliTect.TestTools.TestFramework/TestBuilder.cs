using Microsoft.Extensions.DependencyInjection;
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
            AddLogger<DebugLogger, JsonSerializer>();
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
        // OR
        private List<Block> TestBlocks { get; } = new();
        private List<Block> FinallyBlocks { get; } = new();
        private List<InvalidOperationException> ValidationExceptions { get; } = new();

        private IServiceCollection Services { get; } = new ServiceCollection();
        //private HashSet<object> TestBlockResults { get; } = new HashSet<object>();
        private int TestCaseId { get; set; }
        private string? TestCaseName { get; set; }
        private string TestMethodName { get; set; }
        //private Exception? TestBlockException { get; set; }

        /// <summary>
        /// Adds a test block (some related group of test actions) to the list of blocks to run for any given test case
        /// </summary>
        /// <typeparam name="T">The type of test block, as an ITestBlock, to run</typeparam>
        /// <returns>This</returns>
        public TestBuilder AddTestBlock<T>() where T : ITestBlock
        {
            MethodInfo execute = FindExecuteMethod(typeof(T));
            TestBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: null));
            TestBlocks.Add(new Block(typeof(T), execute));
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
            throw new NotImplementedException("Not yet supported in TestFramework v2.");
            //Block tb = new(typeof(T));
            //tb.ExecuteParams = testBlockArgs;
            //TestBlocks.Add(tb);
            //TestBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: testBlockArgs));
            //Services.AddTransient(typeof(T));
            //return this;
        }

        public TestBuilder AddFinallyBlock<T>() where T : ITestBlock
        {
            MethodInfo execute = FindExecuteMethod(typeof(T));
            FinallyBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: null));
            FinallyBlocks.Add(new Block(typeof(T), execute));
            Services.AddTransient(typeof(T));
            return this;
        }

        public TestBuilder AddFinallyBlock<T>(params object[] testBlockArgs) where T : ITestBlock
        {
            throw new NotImplementedException("Not yet supported in TestFramework v2.");
            //Block tb = new(typeof(T));
            //tb.ExecuteParams = testBlockArgs;
            //FinallyBlocks.Add(tb);
            //FinallyBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: testBlockArgs));
            //Services.AddTransient(typeof(T));
            //return this;
        }

        /// <summary>
        /// Adds a service as a factory a container that is used to fulfill TestBlock dependencies
        /// </summary>
        /// <typeparam name="T">The type of dependency a test block needs to execute</typeparam>
        /// <param name="serviceFactory">The factory to provide an instance of the type needed for a test block to execute</param>
        /// <returns></returns>
        public TestBuilder AddDependencyService<T>(Func<IServiceProvider, object> serviceFactory)
        {
            // When switching to rebuilding a testblock scope, this may need to change to Services.AddSingleton to persist between test blocks.
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
            // When switching to rebuilding a testblock scope, this may need to change to Services.AddSingleton to persist between test blocks.
            Services.AddScoped(typeof(T));
            return this;
        }

        public TestBuilder AddDependencyService<TServiceType, TImplementationType>()
        {
            // When switching to rebuilding a testblock scope, this may need to change to Services.AddSingleton to persist between test blocks.
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

        // We do this twice, once for Logger and once for Serializer
        // May need to figure out how to genericize this
        public TestBuilder AddLogger<T>() where T : ITestCaseLogger
        {
            RemoveLogger();
            Services.AddSingleton(typeof(ITestCaseLogger), typeof(T));
            return this;
        }

        // Is this enough convenience to keep?
        public TestBuilder AddLogger<TLogger, TObjectSerializer>() 
            where TLogger : ITestCaseLogger 
            where TObjectSerializer : IObjectSerializer
        {
            RemoveSerializer();
            RemoveLogger();
            Services.AddSingleton(typeof(ITestCaseLogger), typeof(TLogger));
            Services.AddSingleton(typeof(IObjectSerializer), typeof(TObjectSerializer));
            return this;
        }

        public TestBuilder RemoveLogger()
        {
            ServiceDescriptor? logger = Services.FirstOrDefault(d => d.ServiceType == typeof(ITestCaseLogger));
            if (logger is { }) Services.Remove(logger);
            return this;
        }

        // Maybe don't need this since we have AddLogger<TLogger,TObjectSerializer>()
        // If we remove this, we should consolidate RemoveSerializer into AddLogger<TL, TOS> OR make RemoveSerializer private
        public TestBuilder AddSerializer<T>() where T : IObjectSerializer
        {
            RemoveSerializer();
            Services.AddSingleton(typeof(IObjectSerializer), typeof(T));
            return this;
        }

        public TestBuilder RemoveSerializer()
        {
            ServiceDescriptor? serializer = Services.FirstOrDefault(d => d.ServiceType == typeof(IObjectSerializer));
            if(serializer is { }) Services.Remove(serializer);
            return this;
        }

        public void BuildWithProvider()
        {
            ServiceProvider provider = Services.BuildServiceProvider();
            string result = provider.GetService<string>();
            if (result is null) throw new NullReferenceException();
        }

        public void BuildWithoutProvider()
        {
            ServiceDescriptor desc = new(typeof(string), typeof(string), ServiceLifetime.Scoped);
            ServiceDescriptor? test1 = Services.FirstOrDefault(x => {
                return x.ServiceType == desc.ServiceType;
            });
            if (test1 is null) throw new NullReferenceException();
        }

        public TestCase Build()
        {
            if(string.IsNullOrWhiteSpace(TestCaseName))
            {
                TestCaseName = TestMethodName;
            }

            TestCase testCase = new(TestCaseName!, TestMethodName, TestCaseId, Services);

            // Probably need to profile all of this for performance at some point.
            // Need to make sure if we're running hundreds or thousands of tests that we're not adding significant amount of time to that.

            // NOTE: This does not verify order.
            // If a test blocks asks for something that a subsequent test block returns,
            // this will not catch that. That will fail at runtime.
            // Need to pair with someone to brainstorm this.
            // Could potentially try something like:
            // foreach test block, add to a new list. Check returns foreach test block against that list as it's built?
            List<(Type TestBlock, Type? Output)> outputs = new();
            foreach (Block tb in TestBlocks)
            {
                GatherDependencies(tb, outputs);
                testCase.TestBlocks.Add(tb);
            }

            foreach (Block fb in FinallyBlocks)
            {
                GatherDependencies(fb, outputs);
                testCase.FinallyBlocks.Add(fb);
            }

            if (ValidationExceptions.Count > 0)
            {
                // This might make sense as an AggregateException instead
                //string message = "";
                //foreach(var vm in validationMessages)
                //{
                //    message += "\r\n";
                //    message += vm;
                //}
                //throw new InvalidOperationException(message);
                throw new AggregateException(ValidationExceptions);
            }

            // add test blocks to test case
            // add finally blocks to test case
            // validate inputs and outputs (maybe do that first?)

            Services.AddSingleton(testCase);
            return testCase;
        }

        private static MethodInfo FindExecuteMethod(Type type)
        {
            List<MethodInfo>? executeMethod = type.GetMethods().Where(m => m.Name.ToUpperInvariant() == "EXECUTE").ToList();
            if (executeMethod.Count is not 1)
            {
                throw new InvalidOperationException(
                    $"TestBlock: {type} - There must be one and only one Execute method on a test block.");
            }

            return executeMethod[0];
        }

        private void GatherDependencies(
            Block tb, 
            List<(Type TestBlock, Type? Output)> outputs)
        {
            ConstructorInfo[]? constructors = tb.Type.GetConstructors();

            if (constructors.Length > 1)
            {
                throw new InvalidOperationException(
                    $"TestBlock: {tb.Type} - TestFramework supports zero or one constructors on test blocks.");
            }

            ParameterInfo[]? ctorParams = constructors[0].GetParameters();
            tb.PropertyParams = tb.Type.GetProperties();
            tb.ExecuteParams = tb.ExecuteMethod.GetParameters();
            // Currently do not support Fields. Should we check for them anyway at least to throw?

            List<(Type TestBlock, Type Input)> inputs = new();
            foreach (var c in ctorParams)
            {
                inputs.Add((tb.Type, c.ParameterType));
            }
            foreach (var p in tb.PropertyParams)
            {
                inputs.Add((tb.Type, p.PropertyType));
            }
            foreach (var e in tb.ExecuteParams)
            {
                inputs.Add((tb.Type, e.ParameterType));
            }

            //if (tb.ExecuteArgumentOverrides is not null)
            //{
                // Need to figure out how to handle test block parameters.
                // For now, we'll ditch them for v2 alpha.
                // In the future it may make sense to keep them strictly as overrides:
                // If a test block param is present, always use it for whatever type is matched. If no type is matched, we should fail here.
                // Do we even need this? The couple of cases we've used it has purely been for fewer lines of code, and not actually overriding anything.
                // For simplicity's sake, maybe we just ditch this.
                // Based on the ideas that best practices are to have tests be as simple as feasible,
                // and use POCO class to represent inputs and outputs
                // it's hard to imagine a case where we need this without introducing confusion for the test author.
                //foreach (var p in tb.TestBlockParameters)
                //{
                //    externalDependencies.Add(p.GetType());
                //}
            //}

            foreach ((Type TestBlock, Type Input) i in inputs)
            {
                // Do we also need to match on implementation here?
                ServiceDescriptor desc = new(i.Input, i.Input, ServiceLifetime.Scoped);
                ServiceDescriptor? obj = Services.FirstOrDefault(x => x.ServiceType == desc.ServiceType);
                // Don't think I need this because I'm matching on service type above, and NOT scope.
                //if (obj is null)
                //{
                //    ServiceDescriptor singleDesc = new(i.Input, i.Input, ServiceLifetime.Singleton);
                //    obj = Services.FirstOrDefault(x => x.ServiceType == singleDesc.ServiceType);
                //}
                if (obj is null)
                {
                    Type? output = outputs.FirstOrDefault(o => o.Output == i.Input).Output;

                    if (output is null)
                    {
                        ValidationExceptions.Add(new InvalidOperationException($"TestBuilder error - TestBlock: {i.TestBlock} - Unable to satisfy test block input: {i.Input}."));
                    }
                }
            }

            Type executeReturns = tb.ExecuteMethod.ReturnType;
            if (executeReturns != typeof(void))
            {
                outputs.Add((tb.Type, executeReturns));
            }
        }

        // Legacy support
        //public void ExecuteTestCase()
        //{
        //    TestCase tc = Build();
        //    tc.ExecuteTestCase();
        //}

        //public void ExecuteTestCase()
        //{
        //    #region move to a Build() method and validate all dependencies are satisfied?
        //    ServiceProvider serviceProvider = Services.BuildServiceProvider();
        //    #endregion

        //    using (var testCaseScope = serviceProvider.CreateScope())
        //    {
        //        var logger = testCaseScope.ServiceProvider.GetService<ILogger>();
        //        if (logger != null)
        //        {
        //            logger.TestCaseKey = TestCaseName;
        //            logger.CurrentTestBlock = "N/A";
        //        }

        //        logger?.Info("Starting test case.");

        //        using (var testBlockScope = serviceProvider.CreateScope())
        //        {
        //            foreach (var tb in TestBlocksAndParams)
        //            {
        //                if (logger != null) logger.CurrentTestBlock = tb.TestBlockType.ToString();
        //                // Might be more concise to have these as out method parameters instead of if statements after every one
        //                var testBlockInstance = GetTestBlock(testBlockScope, tb.TestBlockType);
        //                if (TestBlockException != null) break;

        //                SetTestBlockProperties(testBlockScope, testBlockInstance, logger);
        //                if (TestBlockException != null) break;

        //                MethodInfo execute = GetExecuteMethod(testBlockInstance);
        //                if (TestBlockException != null) break;

        //                var executeArgs = GatherTestBlockArguments(testBlockScope, execute, tb);
        //                if (TestBlockException != null) break;

        //                RunTestBlocks(testBlockInstance, execute, executeArgs, logger);
        //                if (TestBlockException != null) break;
        //            }

        //            // Need a much better way to handle Finally exceptions...
        //            Exception tempException = TestBlockException;
        //            TestBlockException = null;
        //            // Extract loop above since it's basically the same for finally blocks?
        //            foreach (var fb in FinallyBlocksAndParams)
        //            {
        //                if (logger != null) logger.CurrentTestBlock = fb.TestBlockType.ToString();
        //                // Might be more concise to have these as out method parameters instead of if statements after every one
        //                // Also these specific ones should not be overwriting TestBlockException
        //                var testBlockInstance = GetTestBlock(testBlockScope, fb.TestBlockType);
        //                if (TestBlockException != null) break;

        //                SetTestBlockProperties(testBlockScope, testBlockInstance, logger);
        //                if (TestBlockException != null) break;

        //                MethodInfo execute = GetExecuteMethod(testBlockInstance);
        //                if (TestBlockException != null) break;

        //                var executeArgs = GatherTestBlockArguments(testBlockScope, execute, fb);
        //                if (TestBlockException != null) break;

        //                RunTestBlocks(testBlockInstance, execute, executeArgs, logger);
        //                if (TestBlockException != null) break;
        //            }
        //            TestBlockException = tempException;
        //        }

        //        if (TestBlockException == null)
        //        {
        //            logger?.Info("Test case finished successfully.");
        //        }
        //        else
        //        {
        //            logger?.Critical($"Test case failed: {TestBlockException}");
        //        }
        //    }

        //    serviceProvider.Dispose();

        //    if (TestBlockException != null)
        //    {
        //        throw new TestCaseException("Test case failed.", TestBlockException);
        //    }
        //}

        //        private static string GetObjectDataAsJsonString(object obj)
        //        {
        //            // JsonSerializer.Serialize has some different throw behavior between versions.
        //            // One version threw an exception that occurred on a property, which happened to be a Selenium WebDriverException.
        //            // In this one specific case, catch all exceptions and move on to provide standard behavior to all package consumers.
        //            // TL;DR: we don't want logging failures to interrupt the test run.
        //            try
        //            {
        //                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        //            }
        //#pragma warning disable CA1031 // Do not catch general exception types
        //            catch (Exception e)
        //#pragma warning restore CA1031 // Do not catch general exception types
        //            {
        //                return $"Unable to serialize object {obj?.GetType()} to JSON. Mark the relevant property with the [JsonIgnore] attribute: {e}";
        //            }
        //        }

        //private object GetTestBlock(IServiceScope scope, Type tbType)
        //{
        //    var tb = scope.ServiceProvider.GetService(tbType);
        //    if (tb == null)
        //    {
        //        TestBlockException = new InvalidOperationException($"Unable to find test block: {tbType.FullName}.");
        //    }

        //    return tb;
        //}

        //private void SetTestBlockProperties(IServiceScope scope, object testBlockInstance, ILogger logger)
        //{
        //    // Populate all of our properties
        //    var properties = testBlockInstance.GetType().GetProperties();
        //    foreach (var prop in properties)
        //    {
        //        if (!prop.CanWrite)
        //        {
        //            logger?.Debug($"Skipping property {prop}. No setter found.");
        //            continue;
        //        }
        //        object propertyValue = scope.ServiceProvider.GetService(prop.PropertyType);
        //        if (propertyValue == null)
        //        {
        //            TestBlockException = new InvalidOperationException($"Unable to find an object or service for property {prop.Name} of type {prop.PropertyType.FullName} on test block {testBlockInstance.GetType()}.");
        //            break;
        //        }

        //        prop.SetValue(testBlockInstance, propertyValue);
        //    }
        //}

        //private MethodInfo GetExecuteMethod(object testBlockInstance)
        //{
        //    List<MethodInfo> methods = testBlockInstance.GetType().GetMethods().Where(m => m.Name.ToUpperInvariant() == "EXECUTE").ToList();
        //    // No longer need to do this check since we now do it in Build()
        //    // UNLESS we end up supporting multiple Execute methods, then there may still be a need to double check at run-time?
        //    if (methods.Count != 1)
        //    {
        //        TestBlockException = new InvalidOperationException($"There can be one and only one Execute method on a test block. " +
        //            $"Please review test block {testBlockInstance.GetType()}.");
        //        return null;
        //    }

        //    return methods[0];
        //}

        //private object[] GatherTestBlockArguments(IServiceScope scope, MethodInfo execute, (Type TestBlockType, object[] TestBlockParameters) tb)
        //{
        //    var executeParams = execute.GetParameters();

        //    object[] executeArgs = new object[executeParams.Length];

        //    // Is this the right order of checking? Or should we prioritize test block results first?
        //    // Initial thought is that if someone is passing in explicit arguments, they probably have a good reason, so we should start there
        //    // Populate and log all of our Execute arguments
        //    if (executeArgs.Length > 0)
        //    {
        //        // We should change this to just match on type instead of length of args
        //        // That way you can override just a single parameter if desired. That seems to be the primary (albeit rare) use case here.
        //        if (tb.TestBlockParameters != null && executeParams.Length == tb.TestBlockParameters.Length)
        //        {
        //            // Eventually need to add more validation around making sure the types match here.
        //            executeArgs = tb.TestBlockParameters;
        //        }
        //        else
        //        {
        //            for (int i = 0; i < executeArgs.Length; i++)
        //            {
        //                object foundResult = TestBlockResults.FirstOrDefault(tbr => tbr.GetType() == executeParams[i].ParameterType)
        //                    ?? scope.ServiceProvider.GetService(executeParams[i].ParameterType);
        //                if (foundResult == null)
        //                {
        //                    TestBlockException = new InvalidOperationException($"Unable to find an object or service for Execute parameter {executeParams[i].Name} of type {executeParams[i].ParameterType.FullName} on test block {tb.TestBlockType.FullName}.");
        //                    break;
        //                }

        //                executeArgs[i] = foundResult;
        //            }
        //        }

        //        // Instead of doing this, might be worth extracting the above for loop into a private method and if that fails, then break out of the foreach we're in now
        //        if (TestBlockException != null) return null;
        //    }
        //    return executeArgs;
        //}

        //private void RunTestBlocks(object testBlockInstance, MethodInfo execute, object[] executeArgs, ILogger logger)
        //{
        //    logger?.Debug($"Starting test block.");
        //    // Log ALL inputs
        //    // Is it worth distinguishing between Properties and Execute args?
        //    PropertyInfo[] props = testBlockInstance.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
        //    object[] allArgs = new object[props.Length + executeArgs.Length];

        //    for (int i = 0; i < props.Length; i++)
        //    {
        //        allArgs[i] = props[i].GetValue(testBlockInstance);
        //    }

        //    executeArgs.CopyTo(allArgs, props.Length);
        //    foreach (var arg in allArgs)
        //    {
        //        logger?.TestBlockInput(GetObjectDataAsJsonString(arg));
        //    }

        //    try
        //    {
        //        var result = execute.Invoke(testBlockInstance, executeArgs);
        //        if (result != null)
        //        {
        //            logger?.TestBlockOutput(GetObjectDataAsJsonString(result));
        //            TestBlockResults.Add(result);
        //        }

        //    }
        //    catch (TargetInvocationException ex)
        //    {
        //        TestBlockException = ex.InnerException;
        //        return;
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        TestBlockException = ex;
        //        return;
        //    }
        //    catch (TargetParameterCountException ex)
        //    {
        //        ex.Data.Add("AdditionalInfo", "Test block failed: Mismatched count between Execute method arguments and supplied dependencies.");
        //        TestBlockException = ex;
        //        return;
        //    }

        //    logger?.Debug($"Test block completed successfully.");
        //}
    }
}
