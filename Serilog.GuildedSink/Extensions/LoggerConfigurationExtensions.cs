using Serilog.Configuration;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.GuildedSink.Extensions;

public static class LoggerConfigurationExtensions
{
    /// <summary>
    /// Send logs to a guilded channel from a webhook.
    /// <remarks>Batches are sent every 5 seconds</remarks>
    /// </summary>
    /// <param name="loggerSinkConfiguration">The logger sink configuration</param>
    /// <param name="webhookUrl">The guilded webhook url</param>
    /// <returns>The logger configuration</returns>
    public static LoggerConfiguration Guilded(this LoggerSinkConfiguration loggerSinkConfiguration, string webhookUrl)
    {
        return Guilded(loggerSinkConfiguration, null, null, webhookUrl, TimeSpan.FromSeconds(5));
    }

    public static LoggerConfiguration Guilded(this LoggerSinkConfiguration loggerSinkConfiguration, string? webhookUsername, Uri? webhookAvatar, string webhookUrl)
    {
        return Guilded(loggerSinkConfiguration, webhookUsername, webhookAvatar, webhookUrl, TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// Send logs to a guilded channel from a webhook.
    /// </summary>
    /// <param name="loggerSinkConfiguration">The logger sink configuration</param>
    /// <param name="webhookUrl">The guilded webhook url</param>
    /// <param name="delayBetweenBatches">The delay between each batches</param>
    /// <returns>The logger configuration</returns>
    public static LoggerConfiguration Guilded(this LoggerSinkConfiguration loggerSinkConfiguration,
                                              string? webhookUsername, Uri? webhookAvatar, string webhookUrl,
                                              TimeSpan delayBetweenBatches)
    {
        var guildedSink = new PeriodicGuildedSink(webhookUsername, webhookAvatar, webhookUrl, delayBetweenBatches);

        var batchingOptions = new PeriodicBatchingSinkOptions
        {
                BatchSizeLimit = 10,
                EagerlyEmitFirstEvent = true,
                QueueLimit = 1000
        };

        var batchingSink = new PeriodicBatchingSink(guildedSink, batchingOptions);

        return loggerSinkConfiguration.Sink(batchingSink);
    }
}