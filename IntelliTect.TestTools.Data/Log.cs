using System;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public class Log
    {
        private readonly LogLevel _LogLevel;
        private readonly EventId _EventId;
        private readonly object _State;
        private readonly Exception _Exception;

        public Log(
            LogLevel logLevel,
            EventId eventId,
            object state,
            Exception exception)
        {
            _LogLevel = logLevel;
            _EventId = eventId;
            _State = state;
            _Exception = exception;
        }
    }
}