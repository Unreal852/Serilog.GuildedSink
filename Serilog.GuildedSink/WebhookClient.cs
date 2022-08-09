using Guilded.Base;
using Guilded.Base.Content;
using RestSharp;
using Serilog;

namespace Serilog.GuildedSink;

public class WebhookClient : BaseGuildedService
{
    public WebhookClient() : base(new Uri(GuildedUrl.Media, "webhooks"))
    {
    }

    public async Task CreateMessageAsync(string webhookUrl, MessageContent message)
    {
        RestResponse<object> restResponse = await ExecuteRequestAsync(new RestRequest(webhookUrl, Method.Post).AddBody(message)).ConfigureAwait(false);
        if (!restResponse.IsSuccessful && restResponse.ErrorMessage is { })
        {
            Log.Warning("Failed to send webhook message to {WebhookUrl}. Reason: {Reason}", webhookUrl, restResponse.ErrorMessage);
            if (restResponse.ErrorException is { })
                Log.Error(restResponse.ErrorException, "Exception");
        }
    }
}