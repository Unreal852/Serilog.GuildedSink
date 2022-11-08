using System.Collections.ObjectModel;
using System.Drawing;
using Guilded.Base;
using Guilded.Base.Embeds;
using Guilded.Webhook;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.GuildedSink;

/// <summary>
/// The guilded sink.
/// </summary>
public class PeriodicGuildedSink : IBatchedLogEventSink
{
    private readonly string   _webhookUsername;
    private readonly Uri?     _webhookAvatar;
    private readonly Uri      _webhookUrl;
    private readonly TimeSpan _delayBetweenBatches;

    public PeriodicGuildedSink(string? webhookUsername, Uri? webhookAvatar, string webhookUrl,
                               TimeSpan delayBetweenBatches)
    {
        _webhookUsername = webhookUsername ?? string.Empty;
        _webhookAvatar = webhookAvatar;
        _webhookUrl = string.IsNullOrWhiteSpace(webhookUrl)
                ? throw new ArgumentException("The webhook url is null or empty")
                : new Uri(webhookUrl);
        _delayBetweenBatches = delayBetweenBatches;
    }

    private GuildedWebhookClient WebhookClient { get; } = new(Array.Empty<string>());

    public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
    {
        var messageContent = new MessageContent
        {
                Username = _webhookUsername,
                Avatar = _webhookAvatar,
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
                embed.AddField("**Exception Type**",
                        logEvent.Exception.GetType().FullName ?? logEvent.Exception.GetType().Name);
                embed.AddField("**Message**", logEvent.Exception.Message);
                if (logEvent.Exception.StackTrace is { })
                    embed.AddField("**Stack Trace**", logEvent.Exception.StackTrace);
            }

            messageContent.Embeds.Add(embed);
        }

        await Task.Delay(_delayBetweenBatches);
        try
        {
            await WebhookClient.CreateMessageAsync(_webhookUrl, messageContent);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to send logs to the configured webhook");
        }
    }

    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
    }
}