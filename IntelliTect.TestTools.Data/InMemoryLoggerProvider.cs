using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public class InMemoryLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, InMemoryLogger> _Loggers
            = new ConcurrentDictionary<string, InMemoryLogger>();

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _Loggers.GetOrAdd(categoryName, s => new InMemoryLogger());
        }

        public ConcurrentDictionary<string, InMemoryLogger> GetLoggers()
        {
            return _Loggers;
        }
    }
}
