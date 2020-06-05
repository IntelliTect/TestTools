using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public class InMemoryLogger : ILogger
    {
        /// <summary>
        /// Collection of all Logs recorded by this InMemoryLogger
        /// </summary>
        public List<Log> Logs { get; } = new List<Log>();

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Logs.Add(new Log(logLevel, eventId, state, exception));
        }

        /// <summary>
        /// InMemoryLogger cannot be disabled. Once created an InMemoryLogger will always return true for enabled.
        /// </summary>
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
