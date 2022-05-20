using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data;

public static class ILoggingBuilderMixins
{
    /// <summary>
    /// Adds the <see cref="InMemoryLoggerProvider"/> to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/>to use.</param>
    /// <returns><see cref="ILoggingBuilder"/> with <see cref="InMemoryLoggerProvider"/> configured.</returns>
    public static ILoggingBuilder AddInMemory(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, InMemoryLoggerProvider>());
        return builder;
    }
}