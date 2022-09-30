using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data;

public class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InMemoryLogger> _Loggers = new();

    public void Dispose()
    {
    }

    /// <summary>
    /// Creates a new <see cref="InMemoryLogger" /> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The instance of <see cref="InMemoryLogger" /> that was created.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return _Loggers.GetOrAdd(categoryName, s => new InMemoryLogger());
    }

    /// <summary>
    /// Provides a ConcurrentDictionary of category names with associated <see cref="InMemoryLogger"/> instance.
    /// </summary>
    /// <returns>ConcurrentDictionary of logger category name with associated
    /// <see cref="InMemoryLogger"/> instance</returns>
    public ConcurrentDictionary<string, InMemoryLogger> GetLoggers()
    {
        return _Loggers;
    }
}