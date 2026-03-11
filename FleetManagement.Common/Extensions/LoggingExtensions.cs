using Microsoft.Extensions.Logging;

namespace FleetManagement.Common.Extensions;

public static class LoggingExtensions
{
    public static ILoggingBuilder AddFleetLogging(this ILoggingBuilder builder, string timestampFormat = "[yyyy-MM-dd HH:mm:ss] ")
    {
        builder.AddSimpleConsole(options =>
        {
            options.TimestampFormat = timestampFormat;
            options.IncludeScopes = false;
            options.SingleLine = false;
        });

        return builder;
    }
}
