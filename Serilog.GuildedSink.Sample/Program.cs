// See https://aka.ms/new-console-template for more information

using Serilog;
using Serilog.Events;
using Serilog.GuildedSink.Extensions;

const int logsAmount = 10;
const string logMessage
        = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
string? guildedWebhookUrl = Environment.GetEnvironmentVariable("GuildedWebhook");


if (string.IsNullOrWhiteSpace(guildedWebhookUrl))
{
    Console.WriteLine("Missing guilded webhook url.");
    Console.ReadKey();
    return;
}

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Guilded(guildedWebhookUrl).CreateLogger();

var random = new Random();
var eventLevels = Enum.GetValues<LogEventLevel>();
for (int i = 0; i < logsAmount; i++)
{
    Log.Write(eventLevels[random.Next(0, eventLevels.Length)], logMessage);
}

try
{
    string? myStr = null;
    int length = myStr.Length;
}
catch (Exception e)
{
    Log.Error(e, "Exception message text {ExceptionName}", e.GetType().FullName);
}

Log.Write(LogEventLevel.Information, "LAST LOG");

Log.CloseAndFlush();

Console.ReadKey();