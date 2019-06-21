using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
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
        public TestBuilder AddTestBlock<T>() where T : ITestBlock
        {
            // Is there a better way to do this so I don't have to store the test block type twice?
            _TestBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: null));
            // Can I do this as part of a transient service? It seems weird to mix Autofac and MS.Extensions.DI this way
            //_Services.AddTransient(typeof(T));
            return this;
        }

        public TestBuilder AddTestBlock<T>(params object[] testBlockArgs) where T : ITestBlock
        {
            _TestBlocksAndParams.Add((TestBlockType: typeof(T), TestBlockParameters: testBlockArgs));
            //_Services.AddTransient(typeof(T));
            return this;
        }

        public TestBuilder AddTestCaseService<T>(Func<IServiceProvider, object> serviceFactory)
        {
            _Services.AddScoped(typeof(T), serviceFactory);
            return this;
        }

        public TestBuilder AddTestCaseService<T>()
        {
            _ServiceTypes.Add(typeof(T));
            //_Services.AddScoped(typeof(T));
            return this;
        }

        public TestBuilder AddDependencyInstance(object objToAdd)
        {
            _Services.AddSingleton(objToAdd.GetType(), objToAdd);
            return this;
        }

        public void ExecuteTestCase()
        {
            #region move to a Build() method and validate all dependencies are satisfied?
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(_Services);
            // Can't figure out how to get property injection to work any other way...
            // This really seems like I'm mixing MS and Autofac in weird ways...
            foreach (var tb in _TestBlocksAndParams)
            {
                containerBuilder.RegisterType(tb.TestBlockType).PropertiesAutowired();
            }
            containerBuilder.RegisterTypes(_ServiceTypes.ToArray()).PropertiesAutowired();
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);
            #endregion

            var logger = serviceProvider.GetService<ILogger>() ?? new Log();
            Exception testBlockException = null;
            using (var scope = serviceProvider.CreateScope())
            {
                HashSet<object> testBlockResults = new HashSet<object>();
                foreach (var tb in _TestBlocksAndParams)
                {
                    logger.Info($"Starting test block {tb.TestBlockType}");
                    object testBlockInstance = null;

                    try
                    {
                        testBlockInstance = scope.ServiceProvider.GetService(tb.TestBlockType);
                    }
                    catch(DependencyResolutionException e)
                    {
                        logger.Error($"Unable to find all dependencies necessary for test block {tb.TestBlockType}. " +
                            $"Verify that all properties, constructors, and Execute method arguments for this test block " +
                            $"have a resovable service, factory, or reference added with the AddTestCaseService or AddDependencyInstance methods");
                        testBlockException = e;
                        break;
                    }

                    if(testBlockInstance == null)
                    {
                        logger.Error($"Unable to find all dependencies necessary for test block {tb.TestBlockType}. " +
                            $"Verify that all properties, constructors, and Execute method arguments for this test block " +
                            $"have a resovable service, factory, or reference added with the AddTestCaseService or AddDependencyInstance methods");
                        testBlockException = new DependencyResolutionException($"No registration found when trying to activate all dependencies for the test block {tb.TestBlockType}");
                        break;
                    }
                    

                    // Log all of our inputs
                    var properties = testBlockInstance.GetType().GetProperties();
                    foreach (var prop in properties)
                    {
                        logger.Debug($"Using property {prop.Name} with data: {GetObjectDataAsJsonString(prop.GetValue(testBlockInstance))}");
                    }

                    MethodInfo execute = tb.TestBlockType.GetMethod("Execute");
                    var executeParams = execute.GetParameters();
                    object[] executeArgs = new object[executeParams.Length];

                    // Populate either with DI or params list.
                    // Not both... at least not yet. That code is a lot more complicated.
                    // E.G. we'd have to check the lenghts, make sure the params list is <= execute args
                    // Then figure out if we params list lines up to the start or end of the execute args
                    // THEN figure out what to do with everything left over...
                    if (tb.TestBlockParameters != null)
                    {
                        if(executeParams.Length != tb.TestBlockParameters.Length)
                        {
                            // Probably don't throw an exception. Instead break out of execution?
                            // We have a chance to resolve this in the "else" case.
                            throw new ArgumentException($"Unable to resolve Execute method arguments for test block {tb.TestBlockType}." +
                                $"Check if parameters passed in through AddTestBlock<T>(params object[]) match the first or last set of parameters on {tb.TestBlockType}.Execute()" +
                                $"E.G.: AddTestBlock<{tb.TestBlockType}>(\"string1\", \"string2\" and {tb.TestBlockType}.Execute(string arg1, string arg2).");
                        }
                        executeArgs = tb.TestBlockParameters;
                    }
                    else
                    {
                        for(int i = 0; i < executeArgs.Length; i++)
                        {
                            // DON'T FORGET!
                            // Need to test for all of these conditions.
                            object foundResult =
                                testBlockResults?.FirstOrDefault(tbr => tbr.GetType() == executeParams[i].ParameterType)
                                ?? scope.ServiceProvider.GetService(executeParams[i].ParameterType)
                                ?? throw new ArgumentNullException("Unable to resolve Execute method arguments");
                            executeArgs[i] = foundResult;
                        }
                    }

                    if (executeArgs.Length > 0)
                    {
                        foreach (var arg in executeArgs)
                        {
                            logger.Debug($"Handing argument into Execute(): {GetObjectDataAsJsonString(arg)}");
                        }
                    }

                    try
                    {
                        var result = execute.Invoke(testBlockInstance, executeArgs);
                        if (result != null)
                        {
                            logger.Debug($"Test block returned... {GetObjectDataAsJsonString(result)}");
                            testBlockResults.Add(result);
                        }

                    }
                    catch (TargetInvocationException e)
                    {
                        logger.Error($"Test block {tb.TestBlockType} failed with the exception: {e.InnerException}.");
                        testBlockException = e.InnerException;
                        break;
                    }

                    // Log stuff here
                }
            }

            // After all logging is finished up and we're ready to finish the test...
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

        private List<(Type TestBlockType, object[] TestBlockParameters)> _TestBlocksAndParams { get; set; } = new List<(Type TestBlockType, object[] TestBlockParameters)>();
        private IServiceCollection _Services { get; set; } = new ServiceCollection();
        private HashSet<Type> _ServiceTypes { get; set; } = new HashSet<Type>();
    }
}
