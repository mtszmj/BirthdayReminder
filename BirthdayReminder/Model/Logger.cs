using Mtszmj.Logger;
using System.Collections.Generic;
using System.Linq;

namespace BirthdayReminder
{
    public static class Logger
    {
        public static ILogger Log { get; }
            = new LoggerBuilder()
                .OfType(LoggerWriterType.Console)
                .WithLevel(LogLevel.Trace)
                .Enabled()
                .WithStorage()
                .Build();

        public static IEnumerable<LogMessage> Messages
        {
            get
            {
                var msgs = Log as IStorage;
                return msgs?.Messages ?? Enumerable.Empty<LogMessage>();
            }
        }
    }
}
