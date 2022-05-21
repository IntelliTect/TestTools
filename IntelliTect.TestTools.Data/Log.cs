using System;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data;

public class Log
{
    /// <summary>
    /// Level of log entry.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Id of the event.
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// Log entry contents.
    /// </summary>
    public object State { get; }

    /// <summary>
    /// The exception for the entry.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Represents a single log event.
    /// </summary>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    public Log(
        LogLevel logLevel,
        EventId eventId,
        object state,
        Exception exception)
    {
        LogLevel = logLevel;
        EventId = eventId;
        State = state ?? throw new ArgumentNullException(nameof(state));
        Exception = exception;
    }
}