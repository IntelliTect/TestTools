using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestCase
    {
        public TestCase(string testCaseName, string testMethodName, int testCaseId, IServiceCollection services)
        {
            TestCaseName = testCaseName;
            TestMethodName = testMethodName;
            TestCaseId = testCaseId;
            ServiceCollection = services;
        }
        // Switch to get; init; when this can be updated to .net5
        // Maybe target .net5 support for v3?
        public string TestCaseName { get; } 
        public string TestMethodName { get; }
        public int TestCaseId { get; }
        public IServiceCollection ServiceCollection { get; }
        public bool ThrowOnFinallyBlockException { get; set; } = true;

        // May make sense to make the below public if it's needed for debugging.
        internal List<Block> TestBlocks { get; set; } = new();
        internal List<Block> FinallyBlocks { get; set; } = new();
        //private bool _DisposedValue;


        // TEMPORARY TO TEST
        private Exception? TestBlockException { get; set; }
        private List<Exception> FinallyBlockExceptions { get; } = new();

        // UNSURE IF NEEDED WITH NEW BUILD METHOD
        //private HashSet<object> TestBlockResults { get; } = new();
        private ITestCaseLogger? Log { get; set; }

        public bool Passed { get; set; }

        public void ExecuteTestCase()
        {
            ServiceProvider services = ServiceCollection.BuildServiceProvider();
            using (var testCaseScope = services.CreateScope())
            {
                Log = testCaseScope.ServiceProvider.GetService<ITestCaseLogger>();
                if (Log is not null)
                {
                    Log.TestCaseKey = TestCaseName;
                    Log.CurrentTestBlock = "N/A";
                }

                Log?.Info($"Starting test case: {TestCaseName}");

                if (TestBlocks is null) throw new NullReferenceException();

                foreach (var tb in TestBlocks)
                {
                    // If we do below, where we add the result back to the container, does it make sense to do...
                    // using (var testBlockScope = Services.CreateScope())?
                    // Then we resolve testblocks from *that* scope?
                    // Will need to test things like:
                    // null return
                    // Duplicate returns from dif test blocks
                    // Interface returns
                    if (Log is not null) Log.CurrentTestBlock = tb.Type.ToString();
                    using (var testBlockScope = services.CreateScope())
                    {
                        if (!GetTestBlock(testBlockScope, tb.Type, false, out var testBlockInstance)) break;
                        if (testBlockInstance is null) throw new NullReferenceException();
                    }













                    
                    if (!GetTestBlock(testCaseScope, tb.Type, false, out var testBlockInstance2)) break;
                    if (testBlockInstance2 is null) throw new NullReferenceException();

                    // AFTER EXECUTING A TEST, DOES THIS MAKE SENSE?
                    // var result = RunTestBlocks()
                    // if (result is not null) Services.Add(result);
                }


                // Do I actually need this inner scope?
                // Everything should be given the correct scope when added in the TestBuilder
                // And honestly, the only 'scoped' things should last the duration of the test case, i.e. inputs
                //using (var testBlockScope = services.CreateScope())
                //{
                //    foreach (var tb in TestBlocksAndParams)
                //    {
                //        if (Log != null) Log.CurrentTestBlock = tb.TestBlockType.ToString();
                //        if (!GetTestBlock(testBlockScope, tb.TestBlockType, false, out var testBlockInstance)) break;

                //        // Might be more concise to have these as out method parameters instead of if statements after every one
                //        //var testBlockInstance = GetTestBlock(testBlockScope, tb.TestBlockType);
                //        //if (TestBlockException != null) break;

                //        SetTestBlockProperties(testBlockScope, testBlockInstance!, Log!);
                //        if (TestBlockException != null) break;

                //        MethodInfo? execute = GetExecuteMethod(testBlockInstance!);
                //        if (TestBlockException != null) break;

                //        var executeArgs = GatherTestBlockArguments(testBlockScope, execute!, tb);
                //        if (TestBlockException != null) break;

                //        RunTestBlocks(testBlockInstance!, execute!, executeArgs!, Log!);
                //        if (TestBlockException != null) break;
                //    }

                //    // We consider a test passed if it makes it through all of its test blocks.
                //    // FinallyBlocks may do things differently depending on if the test failed or not.
                //    Passed = true;

                //    // Need a much better way to handle Finally exceptions...
                //    Exception? tempException = TestBlockException;
                //    TestBlockException = null;
                //    // Extract loop above since it's basically the same for finally blocks?
                //    foreach (var fb in FinallyBlocksAndParams)
                //    {
                //        if (Log != null) Log.CurrentTestBlock = fb.TestBlockType.ToString();
                //        // Might be more concise to have these as out method parameters instead of if statements after every one
                //        // Also these specific ones should not be overwriting TestBlockException
                //        var testBlockInstance = GetTestBlock(testBlockScope, fb.TestBlockType);
                //        if (TestBlockException != null) break;

                //        SetTestBlockProperties(testBlockScope, testBlockInstance, Log!);
                //        if (TestBlockException != null) break;

                //        MethodInfo? execute = GetExecuteMethod(testBlockInstance);
                //        if (TestBlockException != null) break;

                //        var executeArgs = GatherTestBlockArguments(testBlockScope, execute!, fb);
                //        if (TestBlockException != null) break;

                //        RunTestBlocks(testBlockInstance, execute!, executeArgs!, Log!);
                //        if (TestBlockException != null) break;
                //    }
                //    TestBlockException = tempException;
                //}

                if (TestBlockException == null)
                {
                    Log?.Info("Test case finished successfully.");
                }
                else
                {
                    Log?.Critical($"Test case failed: {TestBlockException}");
                }
            }

            services.Dispose();

            if (TestBlockException != null)
            {
                throw new TestCaseException("Test case failed.", TestBlockException);
            }
        }

        private bool GetTestBlock(IServiceScope scope, Type testBlockType, bool isFinallyBlock, out object? testBlock)
        {
            var tb = scope.ServiceProvider.GetService(testBlockType);
            if (tb is null)
            {
                testBlock = null;
                if (isFinallyBlock)
                {
                    FinallyBlockExceptions.Add(new InvalidOperationException($"Unable to find finally block: {testBlockType}"));
                }
                else
                {
                    TestBlockException = new InvalidOperationException($"Unable to find test block: {testBlockType}");
                }
                return false;
            }
            testBlock = tb;
            //SetTestBlockProperties(scope, testBlock);
            return true;
        }

        // Seems like we can actually skip a lot of this since we do most of this logic in Build.
        // Instead, maybe store all of the inputs and simplify this to the foreach?
        // Update: NEVERMIND. In Build, we only type check, we don't actually instantiate any objects.
        //private void SetTestBlockProperties(IServiceScope scope, Block block, object blockInstance)
        //{
        //    if (block.PropertyParams is null) return;
        //    foreach(var prop in block.PropertyParams)
        //    {
        //        if (!prop.CanWrite)
        //        {
        //            Log?.Debug($"Skipping property {prop}. No setter found.");
        //            continue;
        //        }
        //        object propertyValue = scope.ServiceProvider.GetService(prop.PropertyType);
        //        if (propertyValue == null)
        //        {
        //            //TestBlockException = new InvalidOperationException($"Unable to find an object or service for property {prop.Name} of type {prop.PropertyType.FullName} on test block {testBlockInstance.GetType()}.");
        //            break;
        //        }

        //        prop.SetValue(blockInstance, propertyValue);
        //    }

        //    // Populate all of our properties
        //    //PropertyInfo[]? properties = testBlockInstance.GetType().GetProperties();
        //    //foreach (var prop in properties)
        //    //{
        //    //    if (!prop.CanWrite)
        //    //    {
        //    //        Log?.Debug($"Skipping property {prop}. No setter found.");
        //    //        continue;
        //    //    }
        //    //    object propertyValue = scope.ServiceProvider.GetService(prop.PropertyType);
        //    //    if (propertyValue == null)
        //    //    {
        //    //        TestBlockException = new InvalidOperationException($"Unable to find an object or service for property {prop.Name} of type {prop.PropertyType.FullName} on test block {testBlockInstance.GetType()}.");
        //    //        break;
        //    //    }

        //    //    prop.SetValue(blockInstance, propertyValue);
        //    //}
        //}


        // TEMPORARY
        // RE-WRITE ALL OF THIS NOW THAT WE BUILD FIRST
        //private object GetTestBlock(IServiceScope scope, Type tbType)
        //{
        //    var tb = scope.ServiceProvider.GetService(tbType);
        //    if (tb == null)
        //    {
        //        TestBlockException = new InvalidOperationException($"Unable to find test block: {tbType.FullName}.");
        //    }

        //    return tb!;
        //}

        //private void SetTestBlockProperties(IServiceScope scope, object testBlockInstance, ITestCaseLogger logger)
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

        //private MethodInfo? GetExecuteMethod(object testBlockInstance)
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

        //private object[]? GatherTestBlockArguments(IServiceScope scope, MethodInfo execute, (Type TestBlockType, object[] TestBlockParameters) tb)
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

        //private void RunTestBlocks(object testBlockInstance, MethodInfo execute, object[] executeArgs, ITestCaseLogger logger)
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
        //        logger?.TestBlockInput(arg);
        //    }

        //    try
        //    {
        //        var result = execute.Invoke(testBlockInstance, executeArgs);
        //        if (result != null)
        //        {
        //            logger?.TestBlockOutput(result);
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

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_DisposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: dispose managed state (managed objects)
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        //        // TODO: set large fields to null
        //        _DisposedValue = true;
        //    }
        //}

        //// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //// ~TestCase()
        //// {
        ////     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        ////     Dispose(disposing: false);
        //// }

        //public void Dispose()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
