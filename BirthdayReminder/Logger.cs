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
        private static ILogWriter writer = new NullLogWriter();
        public static Mtszmj.Log.Logger Log = new Mtszmj.Log.Logger(writer);
    }
}
