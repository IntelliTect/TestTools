using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace IntelliTect.TestTools.Data
{
    public static class ILoggingBuilderMixins
    {
        public static ILoggingBuilder AddInMemory(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, InMemoryLoggerProvider>());
            return builder;
        }
    }
}
