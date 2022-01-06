using IntelliTect.TestTools.TestFramework.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            //Provider = provider;
        }
        // Switch to get; init; when this can be updated to .net5
        // Maybe target .net5 support for v3?
        public string TestCaseName { get; }
        public string TestMethodName { get; }
        public int TestCaseId { get; }
        public bool ThrowOnFinallyBlockException { get; set; } = true;

        // May make sense to make some of the below public if it's needed for debugging.
        // If so, definitely need to change them to internal/private sets.

        internal List<Block> TestBlocks { get; set; } = new();
        internal List<Block> FinallyBlocks { get; set; } = new();
        internal bool HasLogger { get; set; } = true;

        private ITestCaseLogger? Log { get; set; }
        private IServiceCollection ServiceCollection { get; }
        private Dictionary<Type, object> BlockOutput { get; } = new();
        private Exception? TestBlockException { get; set; }
        private List<Exception> FinallyBlockExceptions { get; } = new();

        public bool Passed { get; set; }

        public void Execute()
        {
            ServiceProvider services = ServiceCollection.BuildServiceProvider();
            using (var testCaseScope = services.CreateScope())
            {
                Log = testCaseScope.ServiceProvider.GetService<ITestCaseLogger>();
                if (Log is not null)
                {
                    //Log.TestCaseKey = TestCaseName;
                    Log.CurrentTestBlock = "N/A";
                }

                Log?.Info($"Starting test case: {TestCaseName}");

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
                    Log?.Info($"Starting test block: {tb.Type}");

                    // Should we un-nest these?
                    // Might be easier to break if any single one fails if we do.
                    // Should we re-nest these and have two out args?
                    if (!TryGetBlock(testCaseScope, tb, out object testBlockInstance)) break;
                    if (!TrySetBlockProperties(testCaseScope, tb, testBlockInstance)) break;
                    if (!TryGetExecuteArguments(testCaseScope, tb, out List<object> executeArgs)) break;

                    Passed = TryRunBlock(tb, testBlockInstance, executeArgs);
                }

                foreach (var fb in FinallyBlocks)
                {
                    if (Log is not null) Log.CurrentTestBlock = fb.Type.ToString();
                    Log?.Info($"Starting finally block: {fb.Type}");

                    if (!TryGetBlock(testCaseScope, fb, out var finallyBlockInstance)) break;
                    if (!TrySetBlockProperties(testCaseScope, fb, finallyBlockInstance)) break;
                    if (!TryGetExecuteArguments(testCaseScope, fb, out List<object> executeArgs)) break;
                    TryRunBlock(fb, finallyBlockInstance, executeArgs);
                }


                // Do we need to do all of the below checks, or can we just do if (Passed) { }?
                if (TestBlockException is null && Passed)
                {
                    Log?.Info("Test case finished successfully.");
                }
                else if ((TestBlockException is not null && Passed)
                    || (TestBlockException is null && !Passed))
                {
                    //TestBlockExceptions.Add(new SystemException("Unknown error occured, please review logs."));
                }
                else
                {
                    Log?.Critical($"Test case failed: {TestBlockException}");
                }
            }

            services.Dispose();

            if (TestBlockException is not null)
            {
                throw new TestCaseException("Test case failed.", TestBlockException);
            }
        }

        // Does it make sense for testBlock to be nullable?
        // On one hand, a return of 'true' implies it will never be null.
        // On the other hand, if we modify this code and accidentaly remove/forgot a 'false' check,
        // it would be nice to be forced to null check.
        // Might be worth setting testBlock to be non-nullable and use temp vars as the nullable type?
        private bool TryGetBlock(IServiceScope scope, Block block, out object blockInstance)
        {
            HandleFinallyBlock(
                block,
                () => Log?.Debug($"Attempting to activate test block: {block.Type}"),
                () => Log?.Debug($"Attempting to activate finally block: {block.Type}")
            );
            bool result = false;
            object? foundBlock = null;
            try
            {
                foundBlock = scope.ServiceProvider.GetService(block.Type);
                // What happens in the below scenario?
                //blockInstance = scope.ServiceProvider.GetService(block.Type);
                if (foundBlock is null)
                {
                    HandleFinallyBlock(
                        block,
                        () => TestBlockException = new InvalidOperationException($"Unable to find test block: {block.Type}"),
                        () => FinallyBlockExceptions.Add(new InvalidOperationException($"Unable to find finally block: {block.Type}"))
                    );
                }
            }
            catch (InvalidOperationException e)
            {
                // Only try to re-build the test block if we get an InvalidOperationException.
                // That implies the block was found but could not be activated.
                // Also... can this message be clearer? Not sure what will make sense to people.
                Log?.Debug($"Unable to activate from DI service, attempting to re-build block: {block.Type}. Original error: {e}");

                _ = TryBuildBlock(scope, block, out foundBlock);
            }

            if(foundBlock is not null)
            {
                blockInstance = foundBlock;
                result = true;
            }
            else
            {
                // Is this the best way to do this?
                // Or should blockInstance be nullable?
                blockInstance = new object();
            }

            return result;
        }

        // Notes for documentation:
        // ... First level dependencies should be validated at build time.
        // ... Second level dependencies are not and can fail.
        // ... This mainly affects objects returned by test blocks.
        // ... Best practice is to add as much as possible via AddDependency methods and *only* return items from test blocks that *have* to be.
        // ... E.G. this is fine:
        // ... ... TestBlock1 - returns bool
        // ... ... TestBlock2 - needs bool
        // ... This starts to get problematic and requires extra attention to ensure it's absolutely necesssary:
        // ... ... TestBlock1 - returns bool
        // ... ... TestBlock2 - needs ObjectA which needs bool
        private bool TryBuildBlock(IServiceScope scope, Block block, out object? blockInstance)
        {
            List<object?> blockParams = new();
            foreach (ParameterInfo? c in block.ConstructorParams)
            {
                object? obj = ActivateObject(scope, block, c.ParameterType, "constructor argument");
                if (obj is null)
                {
                    blockInstance = null;
                    return false;
                }

                blockParams.Add(obj);
            }
            blockInstance = Activator.CreateInstance(block.Type, blockParams);
            return true;
        }

        private bool TrySetBlockProperties(IServiceScope scope, Block block, object blockInstance)
        {
            foreach (PropertyInfo? prop in block.PropertyParams)
            {
                if (!prop.CanWrite)
                {
                    Log?.Debug($"Skipping property {prop}. No setter found.");
                    continue;
                }

                object? obj = ActivateObject(scope, block, prop.PropertyType, "property");
                if (obj is null)
                {
                    // Seems like we need a better way to handle this.
                    if (prop.PropertyType == typeof(ITestCaseLogger))
                    {
                        TestBlockException = null;
                        if(FinallyBlockExceptions.Count > 0)
                        {
                            FinallyBlockExceptions.Remove(FinallyBlockExceptions.Last());
                        }
                        
                        continue;
                    }

                    return false;
                }

                prop.SetValue(blockInstance, obj);
                Log?.TestBlockInput(obj);
            }

            return true;
        }

        private bool TryGetExecuteArguments(IServiceScope scope, Block block, out List<object> executeArgs)
        {
            executeArgs = new List<object>();
            foreach (ParameterInfo? ep in block.ExecuteParams)
            {
                object? obj = null;
                if(block.ExecuteArgumentOverrides.Count > 0)
                {
                    obj = executeArgs.FirstOrDefault(a => a.GetType() == ep.ParameterType);
                    block.ExecuteArgumentOverrides.TryGetValue(obj.GetType(), out obj);
                }

                if(obj is null)
                {
                    obj = ActivateObject(scope, block, ep.ParameterType, "execute method argument");
                }

                if (obj is null)
                {
                    return false;
                }

                executeArgs.Add(obj);
                Log?.TestBlockInput(obj);
            }

            return true;
        }

        private object? ActivateObject(IServiceScope scope, Block block, Type objectType, string targetMember) // Probably need to come up with a better name than 'targetMember'.
        {
            if (!BlockOutput.TryGetValue(objectType, out object? obj))
            {
                try
                {
                    obj = scope.ServiceProvider.GetService(objectType);
                }
                catch (InvalidOperationException e)
                {
                    HandleFinallyBlock(
                        block,
                        () => TestBlockException = new InvalidOperationException(
                            $"Test Block - {block.Type} - Error attempting to activate {targetMember}: {objectType}: {e}"),
                        () => FinallyBlockExceptions.Add(new InvalidOperationException(
                            $"Finally Block = {block.Type} - Error attempting to activate {targetMember}: {objectType}: {e}"))
                    );
                }
            }

            if (obj is null)
            {
                HandleFinallyBlock(
                        block,
                        () => TestBlockException = new InvalidOperationException(
                            $"Test Block - {block.Type} - Unable to find {targetMember}: {objectType}"),
                        () => FinallyBlockExceptions.Add(new InvalidOperationException(
                            $"Finally Block = {block.Type} - Unable to find {targetMember}: {objectType}"))
                    );
            }

            return obj;
        }

        private bool TryRunBlock(Block block, object blockInstance, List<object> executeArgs)
        {
            Log?.Info($"Starting test block {block.Type}");
            bool result = false;
            foreach (var arg in executeArgs)
            {
                Log?.TestBlockInput(arg);
            }

            try
            {
                object? output = block.ExecuteMethod.Invoke(blockInstance, executeArgs.ToArray());
                if (output is not null)
                {
                    Log?.TestBlockOutput(output);
                    BlockOutput.Remove(output.GetType());
                    BlockOutput.Add(output.GetType(), output);
                }
                result = true;
            }
            catch (TargetInvocationException ex)
            {
                TestBlockException = ex.InnerException;
            }
            catch (ArgumentException ex)
            {
                TestBlockException = ex;
            }
            catch (TargetParameterCountException ex)
            {
                ex.Data.Add("AdditionalInfo", "Test block failed: Mismatched count between Execute method arguments and supplied dependencies.");
                TestBlockException = ex;
            }

            if (result) Log?.Debug($"Test block completed successfully.");
            return result;
        }

        private static void HandleFinallyBlock(Block block, Action testBlockAction, Action finallyBlockAction)
        {
            if (block.IsFinallyBlock)
            {
                finallyBlockAction();
            }
            else
            {
                testBlockAction();
            }
        }


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
