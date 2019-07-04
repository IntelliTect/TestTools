using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IntelliTect.TestTools.TestFramework
{
    public class TestBuilder
    {
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
            Services.AddSingleton(objToAdd.GetType(), objToAdd);
            return this;
        }

        public void ExecuteTestCase()
        {
            #region move to a Build() method and validate all dependencies are satisfied?
            var serviceProvider = Services.BuildServiceProvider();
            #endregion

            var logger = serviceProvider.GetService<ILogger>() ?? new Log();
            Exception testBlockException = null;
            using (var scope = serviceProvider.CreateScope())
            {
                HashSet<object> testBlockResults = new HashSet<object>();
                foreach (var tb in TestBlocksAndParams)
                {
                    logger.Info($"---Starting test block {tb.TestBlockType}---");
                    object testBlockInstance = null;

                    try
                    {
                        testBlockInstance = scope.ServiceProvider.GetRequiredService(tb.TestBlockType);
                    }
                    catch(InvalidOperationException ex)
                    {
                        // Probably worth moving these logs outside of the foreach so we don't have to duplicate the message
                        logger.Error($"Unable to find the test block instance OR all dependencies necessary for test block {tb.TestBlockType}. " +
                            $"See error: {ex.Message}");
                        testBlockException = ex;
                        break;
                    }

                    // Populate and log all of our inputs
                    var properties = testBlockInstance.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        if (!prop.CanWrite)
                        {
                            logger.Debug($"Unable to set property {prop}. No setter found.");
                            continue;
                        }
                        object propertyValue;
                        try
                        {
                            propertyValue = scope.ServiceProvider.GetRequiredService(prop.PropertyType);
                        }
                        catch(InvalidOperationException ex)
                        {
                            logger.Error($"Unable to find the test block instance OR all dependencies necessary for test block {tb.TestBlockType}. " +
                            $"See error: {ex.Message}");
                            testBlockException = ex;
                            break;
                        }
                        
                        prop.SetValue(testBlockInstance, propertyValue);
                        logger.Debug($"Using property {prop.Name} with data: {GetObjectDataAsJsonString(prop.GetValue(testBlockInstance))}");
                    }

                    List<MethodInfo> methods = tb.TestBlockType.GetMethods().Where(m => m.Name.ToLower() == "execute").ToList();
                    if(methods.Count != 1)
                    {
                        testBlockException = new InvalidOperationException($"There can be one and only one Execute method on a test block. " +
                            $"Please review test block {tb.TestBlockType}.");
                        break;
                    }

                    MethodInfo execute = methods[0];
                    var executeParams = execute.GetParameters();

                    object[] executeArgs = new object[executeParams.Length];

                    // Is this the right order of checking? Or should we prioritize test block results first?
                    // Initial thought is that if someone is passing in explicit arguments, they probably have a good reason, so we should start there
                    if(executeArgs.Length > 0)
                    {
                        if(tb.TestBlockParameters != null && executeParams.Length == tb.TestBlockParameters.Length)
                        {
                            // Eventually need to add more validation around making sure the types match here.
                            executeArgs = tb.TestBlockParameters;
                        }
                        else
                        {
                            for (int i = 0; i < executeArgs.Length; i++)
                            {
                                object foundResult =
                                    testBlockResults.FirstOrDefault(tbr => tbr.GetType() == executeParams[i].ParameterType)
                                    ?? scope.ServiceProvider.GetService(executeParams[i].ParameterType);
                                if(foundResult == null)
                                {
                                    testBlockException = new InvalidOperationException("Unable to resolve Execute method arguments");
                                    break;
                                }
                                executeArgs[i] = foundResult;
                            }
                        }

                        foreach (var arg in executeArgs)
                        {
                            logger.Debug($"Handing argument into Execute(): {GetObjectDataAsJsonString(arg)}");
                        }
                    }

                    // Instead of doing this, might be worth extracting the above for loop into a private method and if that fails, then break out of the foreach we're in now
                    if (testBlockException != null)
                        break;

                    try
                    {
                        var result = execute.Invoke(testBlockInstance, executeArgs);
                        if (result != null)
                        {
                            logger.Debug($"Test block {tb.TestBlockType} returned... {GetObjectDataAsJsonString(result)}");
                            testBlockResults.Add(result);
                            logger.Info($"---Test block {tb.TestBlockType} completed successfully.---");
                        }

                    }
                    catch (TargetInvocationException e)
                    {
                        logger.Error($"---Test block {tb.TestBlockType} failed with the exception: {e.InnerException}.---");
                        testBlockException = e.InnerException;
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        logger.Error($"---Test block {tb.TestBlockType} failed with the exception: {e}.---");
                        testBlockException = e;
                        break;
                    }
                }
            }

            serviceProvider.Dispose();

            if (testBlockException != null)
                throw testBlockException;
        }

        private string GetObjectDataAsJsonString(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
            }
            catch(JsonSerializationException e)
            {
                return $"Unable to serialize to JSON. Mark the relevant property or constructor with the [JsonIgnore] attribute: {e.Message}";
            }
            
        }

        private List<(Type TestBlockType, object[] TestBlockParameters)> TestBlocksAndParams { get; } = new List<(Type TestBlockType, object[] TestBlockParameters)>();
        private IServiceCollection Services { get; } = new ServiceCollection();
    }
}
