using Mtszmj.Log;

namespace BirthdayReminder
{
    public static class Logger
    {
        public static ILogger Log
            = new LoggerBuilder()
                .OfType(LoggerType.Console)
                .WithLevel(LogLevel.Trace)
                .Enabled()
                .WithoutStorage()
                .Build();
    }
}
