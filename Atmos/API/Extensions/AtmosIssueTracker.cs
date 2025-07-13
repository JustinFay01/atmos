using System.Diagnostics;

namespace API.Extensions;

public static class AtmosIssueTracker
{
    /// <summary>
    /// Sentry wrapper for the Atmos Issue Tracker.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static void UseAtmosIssueTracker(this WebApplicationBuilder builder)
    {
        var sentryConfig = builder.Configuration.GetSection("Sentry");
        var dsn = sentryConfig.GetValue<string>("Dsn");
        var enabled = sentryConfig.GetValue<bool>("Enabled");
        var debug = sentryConfig.GetValue<bool>("Debug");

        if (!enabled)
        {
            Debug.WriteLine("Sentry is disabled in the configuration.");
            return;
        }

        if (string.IsNullOrEmpty(dsn))
        {
            throw new ArgumentException("Sentry DSN is not configured in the application settings.");
        }

        SentrySdk.Init(options =>
        {
            options.Dsn = dsn;
            options.Debug = debug;
            options.SendDefaultPii = true;
        });

        builder.ConfigureSentryLogging();
    }

    private static void ConfigureSentryLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.AddConfiguration(builder.Configuration);
        builder.Logging.AddSentry();
    }
}
