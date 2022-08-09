using System.Collections.ObjectModel;
using System.Drawing;
using Guilded.Base.Content;
using Guilded.Base.Embeds;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.GuildedSink;

/// <summary>
/// The guilded sink.
/// </summary>
public class PeriodicGuildedSink : IBatchedLogEventSink
{
    private readonly string   _webhookUrl;
    private readonly TimeSpan _delayBetweenBatches;

    public PeriodicGuildedSink(string webhookUrl, TimeSpan delayBetweenBatches)
    {
        _webhookUrl = webhookUrl;
        _delayBetweenBatches = delayBetweenBatches;
    }

    private WebhookClient WebhookClient { get; } = new();

    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        var messageContent = new MessageContent
        {
                Embeds = new Collection<Embed>()
        };
        foreach (LogEvent logEvent in batch)
        {
            var embed = new Embed
            {
                    Title = logEvent.Level.ToString(),
                    Timestamp = logEvent.Timestamp.UtcDateTime,
                    Description = logEvent.RenderMessage(),
                    Color = logEvent.Level switch
                    {
                            LogEventLevel.Verbose     => Color.Gray,
                            LogEventLevel.Debug       => Color.DodgerBlue,
                            LogEventLevel.Information => Color.White,
                            LogEventLevel.Warning     => Color.OrangeRed,
                            LogEventLevel.Error       => Color.Red,
                            LogEventLevel.Fatal       => Color.DarkRed,
                            _                         => Color.Black
                    }
            };


            if (logEvent.Exception != null)
            {
                embed.AddField("**Exception Type**", logEvent.Exception.GetType().FullName ?? logEvent.Exception.GetType().Name);
                embed.AddField("**Message**", logEvent.Exception.Message);
                if (logEvent.Exception.StackTrace is { })
                    embed.AddField("**Stack Trace**", logEvent.Exception.StackTrace);
            }

            messageContent.Embeds.Add(embed);
        }

        await Task.Delay(_delayBetweenBatches);
        await WebhookClient.CreateMessageAsync(_webhookUrl, messageContent);
    }

    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
    }
}