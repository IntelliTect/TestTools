using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestCase
    {
        public TestCase(string testCaseName, string testMethodName, int testCaseId, IServiceCollection services/*, ServiceProvider provider*/)
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
        // One of the below two properties will eventually be extraneous.
        public IServiceCollection ServiceCollection { get; }
        //public ServiceProvider Provider { get; }
        public bool ThrowOnFinallyBlockException { get; set; } = true;

        // May make sense to make the below public if it's needed for debugging.
        internal List<Block> TestBlocks { get; set; } = new();
        internal List<Block> FinallyBlocks { get; set; } = new();
        //private bool _DisposedValue;


        // TEMPORARY TO TEST
        private Exception? TestBlockException { get; set; }
        private List<Exception> FinallyBlockExceptions { get; } = new();
        //private HashSet<object> ActivatedDependencies { get; } = new();

        // UNSURE IF NEEDED WITH NEW BUILD METHOD
        // Also check if an array is faster
        private HashSet<object> TestBlockOutput { get; } = new();
        private ITestCaseLogger? Log { get; set; }

        public bool Passed { get; set; }

        public void Execute()
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

                    // Should we un-nest these?
                    // Might be easier to break if any single one fails if we do.
                    if (!TryGetBlock(testCaseScope, tb, out var testBlockInstance)) break;

                    if (testBlockInstance is null) throw new NullReferenceException();
                }

                foreach (var fb in FinallyBlocks)
                {
                    if (Log is not null) Log.CurrentTestBlock = fb.Type.ToString();

                    if (!TryGetBlock(testCaseScope, fb, out var finallyBlockInstance)) break;
                    if (finallyBlockInstance is null) throw new NullReferenceException();
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

                if (TestBlockException is null)
                {
                    Log?.Info("Test case finished successfully.");
                    Passed = true;
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
        private bool TryGetBlock(IServiceScope scope, Block block, out object? testBlock)
        {
            bool result = false;
            try
            {
                testBlock = scope.ServiceProvider.GetService(block.Type);
                if (testBlock is null)
                {
                    HandleFinallyBlock(
                        block,
                        () => TestBlockException = new InvalidOperationException($"Unable to find test block: {block.Type}"),
                        () => FinallyBlockExceptions.Add(new InvalidOperationException($"Unable to find finally block: {block.Type}"))
                    );
                }
                else result = true;
            }
            catch(InvalidOperationException)
            {
                // Only try to re-build the test block if we get an InvalidOperationException.
                // That implies the block was found but could not be activated.
                // Also... can this message be clearer? Not sure what will make sense to people.
                Log?.Debug($"Unable to fetch block {block.Type} from DI service. Attempting to build it by type.");
                if(TryBuildBlock(scope, block, out testBlock)) result = true;
            }

            // Recheck for null in case we missed a check above, even if all of the other
            if(testBlock is not null)
            {
                if (TrySetBlockProperties(scope, block, testBlock))
                {
                    if (TryGetExecuteArguments(/*scope, block, testBlock*/)) result = true;
                }
                
            }
            else
            {
                // Do we need this here? Presumably an error was set in TryBuildBlock.
                // Maybe check for an error and add one if none exists?
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
        private bool TryBuildBlock(IServiceScope scope, Block block, out object? testBlock)
        {
            object[] args = Array.Empty<object>();
            foreach (var c in block.ConstructorParams)
            {
                object? arg = TestBlockOutput.FirstOrDefault(o => o.GetType() == c.ParameterType);

                if(arg is null)
                {
                    try
                    {
                        arg = scope.ServiceProvider.GetService(c.ParameterType);
                    }
                    catch (InvalidOperationException e)
                    {
                        HandleFinallyBlock(
                            block,
                            () => TestBlockException = new InvalidOperationException(
                                $"Test Block - {block.Type} - Error attempting to activate constructor argument: {c.ParameterType}: {e}"),
                            () => FinallyBlockExceptions.Add(new InvalidOperationException(
                                $"Finally Block = {block.Type} - Error attempting to activate constructor argument: {c.ParameterType}: {e}"))
                        );
                    }
                    if (arg is null)
                    {
                        HandleFinallyBlock(
                                block,
                                () => TestBlockException = new InvalidOperationException(
                                    $"Test Block - {block.Type} - Unable to find constructor argument: {c.ParameterType}"),
                                () => FinallyBlockExceptions.Add(new InvalidOperationException(
                                    $"Finally Block = {block.Type} - Unable to find constructor argument: {c.ParameterType}"))
                            );
                        testBlock = null;
                        return false;
                    }
                }
                
                args = args.Concat(new object[] { arg }).ToArray();
            }
            testBlock = Activator.CreateInstance(block.Type, args);
            return true;
        }

        private bool TrySetBlockProperties(IServiceScope scope, Block block, object blockInstance)
        {
            // Need to adopt the same pattern here has TryBuildBlock.
            // Basically, let's check the test results first so we don't have to continually hammer the DI service.
            //bool result = false;
            foreach (var prop in block.PropertyParams)
            {
                if (!prop.CanWrite)
                {
                    Log?.Debug($"Skipping property {prop}. No setter found.");
                    continue;
                }
                // This next chunk of code is exactly the same as TrySetBlockProperties.
                // Extract out into its own 'ActivateDependency' method.
                // Also: convert all TestBlockOutput.FirstOrDefault calls into HashSet.TryGetValue
                object? propertyValue = TestBlockOutput.FirstOrDefault(o => o.GetType() == prop.PropertyType);
                if (propertyValue is null)
                {
                    try
                    {
                        propertyValue = scope.ServiceProvider.GetService(prop.PropertyType);
                    }
                    catch (InvalidOperationException e)
                    {
                        HandleFinallyBlock(
                            block,
                            () => TestBlockException = new InvalidOperationException(
                                $"Test Block - {block.Type} - Error attempting to activate property: {prop.PropertyType}: {e}"),
                            () => FinallyBlockExceptions.Add(new InvalidOperationException(
                                $"Finally Block = {block.Type} - Error attempting to activate constructor argument: {prop.PropertyType}: {e}"))
                        );
                    }
                    if (propertyValue is null)
                    {
                        HandleFinallyBlock(
                                block,
                                () => TestBlockException = new InvalidOperationException(
                                    $"Test Block - {block.Type} - Unable to find constructor argument: {prop.PropertyType}"),
                                () => FinallyBlockExceptions.Add(new InvalidOperationException(
                                    $"Finally Block = {block.Type} - Unable to find constructor argument: {prop.PropertyType}"))
                            );
                        return false;
                    }
                }

                prop.SetValue(blockInstance, propertyValue);
            }

            return true;
        }

        private static bool TryGetExecuteArguments(/*IServiceScope scope, Block block, object blockInstance*/)
        {
            return false;
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
