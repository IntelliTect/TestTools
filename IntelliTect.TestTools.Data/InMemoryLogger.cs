using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public class InMemoryLogger : ILogger
    {
        public List<Log> Logs { get; } = new List<Log>();

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Logs.Add(new Log(logLevel, eventId, state, exception));
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
