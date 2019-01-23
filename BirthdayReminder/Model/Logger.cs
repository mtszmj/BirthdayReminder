using Mtszmj.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BirthdayReminder
{
    public static class Logger
    {
        public static ILogger Log { get; }
            = new LoggerBuilder()
                .OfType(LoggerWriterType.TextFile)
                .WithPath($"logfile_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.log")
                .WithLevel(LogLevel.Trace)
                .Enabled()
                .WithoutStorage()
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
