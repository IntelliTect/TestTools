using System;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public class Log
    {
        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public object State { get; }
        public Exception Exception { get; }

        public Log(
            LogLevel logLevel,
            EventId eventId,
            object state,
            Exception exception)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
        }
    }
}
