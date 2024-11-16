using Serilog;

namespace Devices.API.Startup;

internal static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureHostBuilder(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));

        return hostBuilder;
    }
}