using Serilog.Configuration;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.GuildedSink.Extensions;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration Guilded(this LoggerSinkConfiguration loggerSinkConfiguration, string webhookUrl)
    {
        return Guilded(loggerSinkConfiguration, webhookUrl, TimeSpan.FromSeconds(5));
    }

    public static LoggerConfiguration Guilded(this LoggerSinkConfiguration loggerSinkConfiguration, string webhookUrl, TimeSpan delayBetweenBatches)
    {
        var guildedSink = new PeriodicGuildedSink(webhookUrl, delayBetweenBatches);

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