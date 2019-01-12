using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
