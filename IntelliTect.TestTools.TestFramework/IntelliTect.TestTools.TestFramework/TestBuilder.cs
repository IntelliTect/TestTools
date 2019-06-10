using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);
            #endregion

            var logger = serviceProvider.GetService<ILogger>() ?? new Log();
            using (var scope = serviceProvider.CreateScope())
            {
                foreach(var tb in _TestBlocksAndParams)
                {
                    logger.Info($"Starting test block {tb.TestBlockType}");
                    MethodInfo execute = tb.TestBlockType.GetMethod("Execute");
                    // May need to resolve arguments explicitly here for logging purposes instead of adding the test block and then resolving that...
                    var testBlockInstance = serviceProvider.GetService(tb.TestBlockType);

                    #region get properties manually

                    //var properties = tb.TestBlockType.GetProperties();
                    //foreach (var prop in properties)
                    //{
                    //    testBlockInstance.GetType().GetProperty(prop.Name).SetValue(testBlockInstance, serviceProvider.GetService(prop.GetType()));
                    //}

                    #endregion


                    try
                    {
                        var result = execute.Invoke(testBlockInstance, tb.TestBlockParameters);
                    }
                    catch(Exception e)
                    {
                        // What exceptions need to be caught here?
                        // TargetInvocationException
                        // ???
                    }
                    
                    // Log stuff here
                }
            }

            // After all logging is finished up and we're ready to finish the test...
            serviceProvider.Dispose();
        }

        private List<(Type TestBlockType, object[] TestBlockParameters)> _TestBlocksAndParams { get; set; } = new List<(Type TestBlockType, object[] TestBlockParameters)>();
        private IServiceCollection _Services { get; set; } = new ServiceCollection();
    }
}
