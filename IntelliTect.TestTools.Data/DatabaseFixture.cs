using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public class DatabaseFixture<TDbContext> : IDisposable where TDbContext : DbContext
    {
        private SqliteConnection SqliteConnection { get; }

        private Lazy<DbContextOptions<TDbContext>> Options { get; }

        private IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Evaluated immediately after database is first constructed - useful for seeding database when
        /// used in test collection
        /// </summary>
        public Func<TDbContext, Task> InitializeDatabase { get; set; }

        private Dictionary<(Type, string), object> ConstructorDependencies { get; } = new();

        public DatabaseFixture()
        {
            SqliteConnection = new SqliteConnection("DataSource=:memory:");
            SqliteConnection.Open();

            Options = new Lazy<DbContextOptions<TDbContext>>(() => new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite(SqliteConnection)
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(GetLoggerFactory())
                .Options);
        }

        public void AddDependency<T>(T contextConstructorDependency, string name = null)
        {
            ConstructorDependencies.Add((typeof(T), name ?? ""), contextConstructorDependency);
        }

        /// <summary>
        /// Fired when loggers are being setup. Immediately follows adding the InMemoryLogger
        /// </summary>
        public event EventHandler<ILoggingBuilder> BeforeLoggingSetup;

        private ILoggerFactory GetLoggerFactory()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
            {
                builder.AddInMemory();

                BeforeLoggingSetup?.Invoke(this, builder); ;
            });

            ServiceProvider = serviceCollection.BuildServiceProvider();

            return ServiceProvider
                .GetService<ILoggerFactory>();
        }

        private async Task<TDbContext> CreateNewContext()
        {
            var constructorInfo = typeof(TDbContext)
                .GetConstructors()
                .Where(x =>
                {
                    var parameters = x.GetParameters();
                    if (parameters.Length < 1) return false;
                    if (!parameters.Any(IsDbContextOptions)) return false;
                    if (parameters.Any(p => GetConstructorParameter(p) == null)) return false;
                    return true;
                })
                .OrderByDescending(x => x.GetParameters().Length)
                .FirstOrDefault();

            if (constructorInfo is null)
            {
                throw new InvalidOperationException(
                    $"'{typeof(TDbContext)}' does not contain constructor that has a valid signature");
            }

            bool alreadyCreated = Options.IsValueCreated;

            var db = (TDbContext)constructorInfo.Invoke(GetConstructorValues(constructorInfo));

            if (alreadyCreated) return db;

            await db.Database.EnsureCreatedAsync();

            if (InitializeDatabase is not null)
            {
                await InitializeDatabase(await CreateNewContext());
            }

            return db;

            static bool IsDbContextOptions(ParameterInfo parameter)
                => parameter.ParameterType == typeof(DbContextOptions<>).MakeGenericType(typeof(TDbContext)) ||
                   parameter.ParameterType == typeof(DbContextOptions);

            Func<object> GetConstructorParameter(ParameterInfo parameter)
            {
                if (IsDbContextOptions(parameter))
                {
                    return () => Options.Value;
                }
                else if (ConstructorDependencies.TryGetValue((parameter.ParameterType, parameter.Name), out object typeAndNameMatch))
                {
                    return () => typeAndNameMatch;
                }
                else if (ConstructorDependencies.TryGetValue((parameter.ParameterType, ""), out object typeMatch))
                {
                    return () => typeMatch;
                }
                else
                {
                    return null;
                }
            }

            object[] GetConstructorValues(ConstructorInfo constructor)
            {
                var parameters = constructor.GetParameters();

                var values = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    Func<object> constructorValue = GetConstructorParameter(parameters[i]);
                    if (constructorValue != null)
                    {
                        values[i] = constructorValue();
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }

                return values;
            }
        }

        /// <summary>
        /// Creates new instance of TDBContext and executes database operation.
        /// This avoids issues where reusing the same DbContext can result in cached objects being returned,
        /// suppressing issues with your LINQ to SQL code.
        /// At minimum the Arrange/Act/Assert portions should each invoke this method separately, but more invocations
        /// of this method should be preferred whenever possible.
        /// </summary>
        /// <param name="operation">The database operation to be performed</param>
        public async Task PerformDatabaseOperation(Func<TDbContext, Task> operation)
        {
            var db = await CreateNewContext();
            await operation(db);
        }

        /// <summary>
        /// If <see cref="InMemoryLogger"/> is configured and DbContext has been accessed at least once, returns a
        /// Dictionary of all InMemoryLoggers
        /// </summary>
        /// <returns>Dictionary with category name, and instance of all InMemoryLoggers</returns>
        /// <exception cref="InvalidOperationException">InMemoryLogger is not configured, or DbContext has not yet
        /// been accessed</exception>
        public ConcurrentDictionary<string, InMemoryLogger> GetInMemoryLoggers()
        {
            if (ServiceProvider is null)
            {
                throw new InvalidOperationException("ServiceCollection is not yet initialized. " +
                                                    "Perform some database operation to initialize loggers");
            }

            if (!(ServiceProvider.GetService<ILoggerProvider>() is InMemoryLoggerProvider loggerProvider))
            {
                throw new InvalidOperationException($"{typeof(ILoggerProvider).FullName} of type " +
                                                    $"{typeof(InMemoryLoggerProvider).FullName} could not be found.");
            }

            return loggerProvider.GetLoggers();
        }

        public void Dispose()
        {
            SqliteConnection?.Dispose();
        }
    }
}
