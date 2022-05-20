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

namespace IntelliTect.TestTools.Data;

public class DatabaseFixture<TDbContext1, TDbContext2, TDbContext3, TDbContext4> 
    : DatabaseFixture<TDbContext1, TDbContext2, TDbContext3>
    where TDbContext1 : DbContext
    where TDbContext2 : DbContext
    where TDbContext3 : DbContext
    where TDbContext4 : DbContext
{
    public DatabaseFixture()
    {
        SqliteConnection = new SqliteConnection("DataSource=:memory:");
        SqliteConnection.Open();

        Options = BuildOptions(typeof(TDbContext1), typeof(TDbContext2), typeof(TDbContext3));
    }
}

public class DatabaseFixture<TDbContext1, TDbContext2, TDbContext3> : DatabaseFixture<TDbContext1, TDbContext2>
    where TDbContext1 : DbContext
    where TDbContext2 : DbContext
    where TDbContext3 : DbContext
{
    public DatabaseFixture()
    {
        SqliteConnection = new SqliteConnection("DataSource=:memory:");
        SqliteConnection.Open();

        Options = BuildOptions(typeof(TDbContext1), typeof(TDbContext2), typeof(TDbContext3));
    }
}

public class DatabaseFixture<TDbContext1, TDbContext2> : DatabaseFixture<TDbContext1>
    where TDbContext1 : DbContext
    where TDbContext2 : DbContext
{
    public DatabaseFixture()
    {
        SqliteConnection = new SqliteConnection("DataSource=:memory:");
        SqliteConnection.Open();

        Options = BuildOptions(typeof(TDbContext1), typeof(TDbContext2));
    }
    
    public Task PerformDatabaseOperation<T>(Func<T, Task> operation) where T : DbContext
        => NewContextAndExecute(operation);
    
    public async Task PerformDatabaseOperation<T>(Action<T> operation) where T : DbContext
    {
        Func<T, Task> func = context =>
        {
            operation(context);
            return Task.CompletedTask;
        };

        await NewContextAndExecute(func);
    }
}

public class DatabaseFixture<TDbContext> : IDisposable where TDbContext : DbContext
{
    protected SqliteConnection SqliteConnection { get; set; }

    internal Dictionary<Type, ContextConstructionInfo> Options { get; set; }

    private IServiceProvider ServiceProvider { get; set; }

    private bool _SeedComplete;
    private Func<Task> _Seed;

    /// <summary>
    /// Set seed function to be used on database initialization
    /// </summary>
    public void SetInitialize<T>(Func<T, Task> seed) where T : DbContext
    {
        GetOrAddConstructionInfo<T>();

        _Seed = async () =>
        {
            var db = await CreateNewContext<T>();
            await seed(db);
        };
    }

    private Dictionary<(Type, string), object> ConstructorDependencies { get; } = new();

    public DatabaseFixture()
    {
        SqliteConnection = new SqliteConnection("DataSource=:memory:");
        SqliteConnection.Open();

        Options = BuildOptions(typeof(TDbContext));
    }

    internal Dictionary<Type, ContextConstructionInfo> BuildOptions(params Type[] types)
    {
        return types
            .Select(BuildContextConstructionInfo)
            .ToDictionary(x => x.DbContextType, x => x);
    }

    internal ContextConstructionInfo BuildContextConstructionInfo(Type t)
    {
        var optionsBuilderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(t);
        var dbContextOptions = (DbContextOptionsBuilder)Activator.CreateInstance(optionsBuilderType);

        var lazyType = typeof(Lazy<>).MakeGenericType(typeof(DbContextOptions));
        var funcType = typeof(Func<>).MakeGenericType(typeof(DbContextOptions));

        var lazyObjectConstructor = lazyType
            .GetConstructors()
            .Where(xi => xi.GetParameters().Length == 1)
            .Single(xi => xi.GetParameters().Single().ParameterType == funcType);

        Func<DbContextOptions> func = () => dbContextOptions
            .UseSqlite(SqliteConnection)
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(GetLoggerFactory())
            .Options;

        var lazy = (Lazy<DbContextOptions>)lazyObjectConstructor.Invoke(new object[] { func });
        var contextInfo = new ContextConstructionInfo
        {
            Lazy = lazy,
            DbContextType = t
        };
        return contextInfo;
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
    
    private static bool IsDbContextOptions<T>(ParameterInfo parameter)
        => parameter.ParameterType == typeof(DbContextOptions<>).MakeGenericType(typeof(T)) ||
           parameter.ParameterType == typeof(DbContextOptions);

    private ConstructorInfo GetConstructorInfo<T>()
    {
        var ourOptions = Options[typeof(T)];

        if (ourOptions.ConstructorInfo is { } ctorInfo) return ctorInfo;

        var constructorInfo = typeof(T)
            .GetConstructors()
            .Where(x =>
            {
                var parameters = x.GetParameters();
                if (parameters.Length < 1) return false;
                return parameters.Any(IsDbContextOptions<T>) 
                       && parameters.All(p => GetConstructorParameter<T>(p) != null);
            })
            .OrderByDescending(x => x.GetParameters().Length)
            .FirstOrDefault();

        if (constructorInfo is null)
        {
            throw new InvalidOperationException(
                $"'{typeof(T)}' does not contain constructor that has a valid signature");
        }

        ourOptions.ConstructorInfo = constructorInfo;

        return constructorInfo;
    }

    private Func<object> GetConstructorParameter<T>(ParameterInfo parameter)
    {
        var ourOptions = Options[typeof(T)];
        
        if (IsDbContextOptions<T>(parameter))
        {
            return () => ourOptions.Lazy.Value;
        }
        if (ConstructorDependencies.TryGetValue(
                (parameter.ParameterType, parameter.Name),
                out object typeAndNameMatch))
        {
            return () => typeAndNameMatch;
        }
        return ConstructorDependencies.TryGetValue((parameter.ParameterType, ""), out object typeMatch) 
            ? () => typeMatch 
            : null;
    }

    internal ContextConstructionInfo GetOrAddConstructionInfo<T>()
    {
        var type = typeof(T);
        if (Options.ContainsKey(type)) return Options[type];
        
        var newOptions = BuildOptions(type).Single().Value;
        Options.Add(type, newOptions);
        return newOptions;
    }
    
    private async Task<T> CreateNewContext<T>() where T : DbContext
    {
        ContextConstructionInfo ourOptions = GetOrAddConstructionInfo<T>();

        var alreadyCreated = ourOptions.Lazy.IsValueCreated;
        
        var ctorInfo = GetConstructorInfo<T>();

        var db = (T)ctorInfo.Invoke(GetConstructorValues(ctorInfo));

        if (alreadyCreated) return db;

        await db.Database.EnsureCreatedAsync();

        if (_SeedComplete) return db;

        _Seed?.Invoke();
        _SeedComplete = true;

        return db;

        object[] GetConstructorValues(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();

            var values = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                Func<object> constructorValue = GetConstructorParameter<T>(parameters[i]);
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
        await NewContextAndExecute(operation);
    }

    /// <summary>
    /// Creates new instance of TDBContext and executes database operation.
    /// This avoids issues where reusing the same DbContext can result in cached objects being returned,
    /// suppressing issues with your LINQ to SQL code.
    /// At minimum the Arrange/Act/Assert portions should each invoke this method separately, but more invocations
    /// of this method should be preferred whenever possible.
    /// </summary>
    /// <param name="operation">The database operation to be performed</param>
    public async Task PerformDatabaseOperation(Action<TDbContext> operation)
    {
        Func<TDbContext, Task> func = context =>
        {
            operation(context);
            return Task.CompletedTask;
        };

        await NewContextAndExecute(func);
    }

    protected async Task NewContextAndExecute<T>(Func<T, Task> operation) where T : DbContext
    {
        var db = await CreateNewContext<T>();
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