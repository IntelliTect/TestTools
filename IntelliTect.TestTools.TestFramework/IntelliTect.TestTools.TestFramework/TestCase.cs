using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        }
        // Switch to get; init; when this can be updated to .net5
        // Maybe target .net5 support for v3?
        /// <summary>
        /// The friendly name for the test case.
        /// </summary>
        public string TestCaseName { get; }
        /// <summary>
        /// The unit test method name. Defaults to the calling member for new TestBuilder().
        /// </summary>
        public string TestMethodName { get; }
        /// <summary>
        /// Any ID associated with the test case, otherwise 0.
        /// </summary>
        public int TestCaseId { get; }
        /// <summary>
        /// If this test case should throw if a finally block has an exception. Defaults to true.
        /// <br />
        /// If a finally block fails and this property is true, the test case is still considered passed internally, but most unit test frameworks will mark the test failed.
        /// </summary>
        public bool ThrowOnFinallyBlockException { get; set; } = true;

        // May make sense to make some of the below public if it's needed for debugging.
        // If so, definitely need to change them to internal or private sets.

        internal List<Block> TestBlocks { get; set; } = new();
        internal List<Block> FinallyBlocks { get; set; } = new();
        internal bool HasLogger { get; set; } = true;

        private ITestCaseLogger? Log { get; set; }
        private IServiceCollection ServiceCollection { get; }
        private Dictionary<Type, object> BlockOutput { get; } = new();
        private Exception? TestBlockException { get; set; }
        private List<Exception> FinallyBlockExceptions { get; } = new();

        // Has this test case passed? Will only be true if every regular test block succeeds.
        public bool Passed { get; set; }

        /// <summary>
        /// Executes the test case.
        /// </summary>
        /// <exception cref="TestCaseException">The exception describing a test failure.</exception>
        /// <exception cref="AggregateException">Occurs when finally blocks fail, or the test fails and at least one finally block fails.</exception>
        public void Execute()
        {
            ServiceProvider services = ServiceCollection.BuildServiceProvider();
            using (var testCaseScope = services.CreateScope())
            {
                Log = testCaseScope.ServiceProvider.GetService<ITestCaseLogger>();
                if (Log is not null)
                {
                    Log.CurrentTestBlock = "N/A";
                }

                Log?.Info($"Starting test case: {TestCaseName}");

                foreach (var tb in TestBlocks)
                {
                    if (Log is not null) Log.CurrentTestBlock = tb.Type.ToString();
                    Log?.Debug($"Starting test block: {tb.Type}");

                    if (!TryGetBlock(testCaseScope, tb, out object testBlockInstance)) break;
                    if (!TrySetBlockProperties(testCaseScope, tb, testBlockInstance)) break;
                    if (!TryGetExecuteArguments(testCaseScope, tb, out List<object?> executeArgs)) break;

                    if (!TryRunBlock(tb, testBlockInstance, executeArgs))
                    {
                        if (TestBlockException is null)
                        {
                            TestBlockException = new(
                                $"Unknown error occurred while running test block {tb}. " +
                                "Please file an issue: https://github.com/IntelliTect/TestTools/issues");
                        }
                        break;
                    }
                }

                foreach (var fb in FinallyBlocks)
                {
                    if (Log is not null) Log.CurrentTestBlock = fb.Type.ToString();
                    Log?.Debug($"Starting finally block: {fb.Type}");

                    if (!TryGetBlock(testCaseScope, fb, out var finallyBlockInstance)) continue;
                    if (!TrySetBlockProperties(testCaseScope, fb, finallyBlockInstance)) continue;
                    if (!TryGetExecuteArguments(testCaseScope, fb, out List<object?> executeArgs)) continue;

                    if (!TryRunBlock(fb, finallyBlockInstance, executeArgs))
                    {
                        Log?.Critical($"Finally block failed: {FinallyBlockExceptions.LastOrDefault()}");
                    }
                }

                if (TestBlockException is null)
                {
                    Passed = true;
                    Log?.Info("Test case finished successfully.");
                }
                else
                {
                    Log?.Critical($"Test case failed: {TestBlockException}");
                }
            }

            services.Dispose();

            // This seems... gross. Revisit after sleeping.
            // Maybe call out in a code review.
            if (TestBlockException is not null
                && (!ThrowOnFinallyBlockException
                || !FinallyBlockExceptions.Any()))
            {
                throw new TestCaseException("Test case failed.", TestBlockException);
            }
            else if(TestBlockException is not null
                && ThrowOnFinallyBlockException
                && FinallyBlockExceptions.Any())
            {
                FinallyBlockExceptions.Insert(0, TestBlockException);
                throw new AggregateException("Test case failed and finally blocks failed.",
                    FinallyBlockExceptions);
            }
            else if(TestBlockException is null
                && ThrowOnFinallyBlockException
                && FinallyBlockExceptions.Any())
            {
                throw new AggregateException("Test case succeeded, but one or more finally blocks failed.",
                        FinallyBlockExceptions);
            }

            //if (tce is not null) throw tce;
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
                    if(!CheckForITestLogger(c.ParameterType))
                    {
                        blockInstance = null;
                        return false;
                    }
                }

                blockParams.Add(obj);
            }
            blockInstance = Activator.CreateInstance(block.Type,
                BindingFlags.CreateInstance |
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.OptionalParamBinding,
                null,
                blockParams.ToArray(),
                CultureInfo.CurrentCulture);

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
                    if(CheckForITestLogger(prop.PropertyType))
                    {
                        continue;
                    }

                    return false;
                }

                prop.SetValue(blockInstance, obj);
                Log?.TestBlockInput(obj);
            }

            return true;
        }

        private bool TryGetExecuteArguments(IServiceScope scope, Block block, out List<object?> executeArgs)
        {
            executeArgs = new List<object?>();
            foreach (ParameterInfo? ep in block.ExecuteParams)
            {
                object? obj = null;
                if(block.ExecuteArgumentOverrides.Count > 0)
                {
                    block.ExecuteArgumentOverrides.TryGetValue(ep.ParameterType, out obj);
                }

                if(obj is null)
                {
                    obj = ActivateObject(scope, block, ep.ParameterType, "execute method argument");
                    if (obj is null)
                    {
                        if (CheckForITestLogger(ep.ParameterType))
                        {
                            executeArgs.Add(null);
                            continue;
                        }

                        return false;
                    }
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
                    // Is the below check worth it?
                    // It is avoided if the test block asks for an interface if the dependency is implementing an interface.
                    // HOWEVER, this would facilitate injecting multiple different implementations in a test.
                    if(obj is null)
                    {
                        foreach(var i in objectType.GetInterfaces())
                        {
                            IEnumerable<object?> objs = scope.ServiceProvider.GetServices(i);
                            obj = objs.FirstOrDefault(o => o?.GetType() == objectType);
                            if (obj is not null) break;
                        }
                    }
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

            // If we've already set an exception, i.e. the GetService call above failed, don't override it.
            // This is to account for two different scenarios: a dependency is not present (below) vs. a dependency is present but failed to activate (above).
            if (obj is null && TestBlockException is null)
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

        private bool TryRunBlock(Block block, object blockInstance, List<object?> executeArgs)
        {
            bool result = false;

            HandleFinallyBlock(
                block,
                () => Log?.Debug($"Executing test block: {block.Type}"),
                () => Log?.Debug($"Executing finally block: {block.Type}")
            );

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
                HandleFinallyBlock(
                    block,
                    () => TestBlockException = ex.InnerException,
                    () => FinallyBlockExceptions.Add(ex.InnerException)
                );
            }
            catch (ArgumentException ex)
            {
                HandleFinallyBlock(
                    block,
                    () => TestBlockException = ex,
                    () => FinallyBlockExceptions.Add(ex)
                );
            }
            catch (TargetParameterCountException ex)
            {
                ex.Data.Add("AdditionalInfo", "Test block failed: Mismatched count between Execute method arguments and supplied dependencies.");
                HandleFinallyBlock(
                    block,
                    () => TestBlockException = ex,
                    () => FinallyBlockExceptions.Add(ex)
                );
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

        // Type checking every single time we find no dependency seems a bit inefficient.
        // Call this out specifically in a code review.
        private bool CheckForITestLogger(Type type)
        {
            bool isLogger = false;
            if (!HasLogger && type == typeof(ITestCaseLogger))
            {
                TestBlockException = null;
                if (FinallyBlockExceptions.Count > 0)
                {
                    FinallyBlockExceptions.Remove(FinallyBlockExceptions.Last());
                }
                isLogger = true;
            }
            return isLogger;
        }
    }
}
