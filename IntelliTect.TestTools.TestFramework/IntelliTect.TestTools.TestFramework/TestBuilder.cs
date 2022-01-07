using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

        private string? TestCaseName { get; set; }
        private string TestMethodName { get; set; }
        private int TestCaseId { get; set; }
        private List<Block> TestBlocks { get; } = new();
        private List<Block> FinallyBlocks { get; } = new();
        private IServiceCollection Services { get; } = new ServiceCollection();
        private bool HasLogger { get; set; } = true;
        private List<Exception> ValidationExceptions { get; } = new();

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

        /// <summary>
        /// Adds a test block (some related group of test actions) with an optional list of arguments. 
        /// Any argument passed here will override all other matched arguments for the blocks TestBlock.Execute() method.
        /// </summary>
        /// <typeparam name="T">The type of dependency a test block needs to execute</typeparam>
        /// <param name="testBlockArgs">The list of arguments to fulfill a set of Execute(params object[]) parameters</param>
        /// <returns>This</returns>
        public TestBuilder AddTestBlock<T>(params object[] testBlockArgs) where T : ITestBlock
        {
            Block tb = CreateBlock<T>(false, testBlockArgs);
            TestBlocks.Add(tb);
            return this;
        }

        public TestBuilder AddFinallyBlock<T>(params object[] finallyBlockArgs) where T : ITestBlock
        {
            Block fb = CreateBlock<T>(true, finallyBlockArgs);
            FinallyBlocks.Add(fb);
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
            Services.AddSingleton(typeof(T), serviceFactory);
            return this;
        }

        /// <summary>
        /// Adds a service as a Type to the container that is used to fulfill TestBlock dependencies
        /// </summary>
        /// <typeparam name="T">The type of test block, as an ITestBlock, to run</typeparam>
        /// <returns>This</returns>
        public TestBuilder AddDependencyService<T>()
        {
            Services.AddSingleton(typeof(T));
            return this;
        }

        public TestBuilder AddDependencyService<TServiceType, TImplementationType>()
        {
            Services.AddSingleton(typeof(TServiceType), typeof(TImplementationType));
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
            Services.AddSingleton(objToAdd.GetType(), objToAdd);
            return this;
        }

        public TestBuilder AddDependencyInstance<T>(object objToAdd)
        {
            Services.AddSingleton(typeof(T), objToAdd);
            return this;
        }

        public TestBuilder AddLogger<T>() where T : ITestCaseLogger
        {
            RemoveLogger();
            Services.AddSingleton(typeof(ITestCaseLogger), typeof(T));
            HasLogger = true;
            return this;
        }

        public TestBuilder RemoveLogger()
        {
            ServiceDescriptor? logger = Services.FirstOrDefault(d => d.ServiceType == typeof(ITestCaseLogger));
            if (logger is { }) Services.Remove(logger);
            HasLogger = false;
            return this;
        }

        // Legacy support. Will get removed before final version is pushed out.
        public void ExecuteTestCase()
        {
            TestCase tc = Build();
            tc.Execute();
        }

        public TestCase Build()
        {
            if (string.IsNullOrWhiteSpace(TestCaseName))
            {
                TestCaseName = TestMethodName;
            }

            TestCase testCase = new(TestCaseName!, TestMethodName, TestCaseId, Services);
            Services.AddSingleton(testCase);

            // Probably need to profile all of this for performance at some point.
            // Need to make sure if we're running hundreds or thousands of tests that we're not adding significant amount of time to that.

            List<Type?> outputs = new();
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

            CheckContainer(outputs/*, provider*/);

            if (ValidationExceptions.Count > 0)
            {
                throw new AggregateException(ValidationExceptions);
            }

            return testCase;
        }

        private Block CreateBlock<T>(bool isFinally, params object[] args)
        {
            Services.AddTransient(typeof(T));

            MethodInfo execute = FindExecuteMethod(typeof(T));
            Block b = new(typeof(T), execute);
            foreach (object? a in args)
            {
                // What happens if the value is null?
                // May need to null check.
                // Might need to better handle adding duplicates than the default Dictionary error
                if(b.ExecuteArgumentOverrides.ContainsKey(a.GetType()))
                {
                    ValidationExceptions.Add(new ArgumentException($"An item with the same key has already been added. Key: {a.GetType()}"));
                }
                else
                {
                    b.ExecuteArgumentOverrides.Add(a.GetType(), a);
                }
            }
            b.IsFinallyBlock = isFinally;
            return b;
        }

        private static MethodInfo FindExecuteMethod(Type type)
        {
            List<MethodInfo>? executeMethod = type.GetMethods().Where(m => m.Name.ToUpperInvariant() == "EXECUTE").ToList();
            if (executeMethod.Count is not 1)
            {
                // Don't add to validation messages we don't have any reasonable assurance what the dependencies should be.
                throw new InvalidOperationException(
                    $"TestBlock: {type} - There must be one and only one Execute method on a test block.");
            }

            return executeMethod[0];
        }

        private void GatherDependencies(
            Block tb,
            List<Type?> outputs)
        {
            ConstructorInfo[]? constructors = tb.Type.GetConstructors();

            if (constructors.Length > 1)
            {
                // Don't add to validation messages we don't have any reasonable assurance what the dependencies should be.
                throw new InvalidOperationException(
                    $"TestBlock: {tb.Type} - TestFramework supports zero or one constructors on test blocks.");
            }

            tb.ConstructorParams = constructors[0].GetParameters();
            tb.PropertyParams = tb.Type.GetProperties();
            tb.ExecuteParams = tb.ExecuteMethod.GetParameters();
            // Currently do not support Fields. Should we check for them anyway at least to throw?

            HashSet<Type> inputs = new();
            foreach (var c in tb.ConstructorParams)
            {
                inputs.Add(c.ParameterType);
            }
            foreach (var p in tb.PropertyParams)
            {
                if(p.CanWrite)
                {
                    inputs.Add(p.PropertyType);
                }
            }
            foreach (var e in tb.ExecuteParams)
            {
                inputs.Add(e.ParameterType);
            }

            if (!HasLogger) inputs.RemoveWhere(i => i == typeof(ITestCaseLogger));
            if (tb.ExecuteArgumentOverrides is not null)
            {
                if (tb.ExecuteArgumentOverrides.Count > tb.ExecuteParams.Length)
                {
                    ValidationExceptions.Add(new ArgumentException($"TestBuilder error - TestBlock: {tb.Type} - Too many execute overrides were provided. More were handed in than parameters on Execute method."));
                }
                else
                {
                    foreach (KeyValuePair<Type, object> eao in tb.ExecuteArgumentOverrides)
                    {
                        if (!tb.ExecuteParams.Any(ep => ep.ParameterType == eao.Key))
                        {
                            ValidationExceptions.Add(new ArgumentException($"TestBuilder error - TestBlock: {tb.Type} - Unable to find corresponding Execute parameter for override argument {eao}"));
                        }
                        else
                        {
                            // Input is satisfied by execute argument override.
                            // No need to check later.
                            inputs.Remove(eao.Key);
                        }
                    }
                }
            }

            foreach (Type i in inputs)
            {
                // Do we also need to match on implementation here?
                ServiceDescriptor desc = new(i, i, ServiceLifetime.Scoped);
                CheckContainerForFirstLevelDependency(desc, outputs, $"TestBuilder error - TestBlock: {tb.Type} - Unable to satisfy test block input: {i}.");
            }

            Type executeReturns = tb.ExecuteMethod.ReturnType;
            if (executeReturns != typeof(void))
            {
                outputs.Add(executeReturns);
            }
        }

        private void CheckContainer(List<Type?> outputs/*, ServiceProvider provider*/)
        {
            foreach (var s in Services)
            {
                CheckContainerForFirstLevelDependency(s, outputs, $"TestBuilder error - ServiceContainer - Unable to satisfy service: {s}.");
            }
        }

        private void CheckContainerForFirstLevelDependency(ServiceDescriptor desc, List<Type?> outputs, string errorMessage)
        {
            // This will NOT check that dependencies of dependencies are satisfied.
            ServiceDescriptor? obj = Services.FirstOrDefault(x => x.ServiceType == desc.ServiceType || x.ImplementationType == desc.ImplementationType);
            if (obj is null)
            {
                Type? output = outputs.FirstOrDefault(o => o == desc.ServiceType || o == desc.ImplementationType);

                if (output is null)
                {
                    if (desc.ServiceType is ITestCaseLogger) return;
                    ValidationExceptions.Add(new InvalidOperationException(errorMessage));
                }
            }
        }
    }
}
