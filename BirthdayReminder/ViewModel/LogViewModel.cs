using Mtszmj.Log;
using System.Collections.ObjectModel;
using System.Windows;

namespace BirthdayReminder
{
    public class LogViewModel : ViewModelBase
    {
        public ObservableCollection<LogMessage> Logs { get; set; } = new ObservableCollection<LogMessage>();

        public LogViewModel()
        {
            Logger.Log.OnMessageLogged += Log_OnMessageLogged;
        }

        private void Log_OnMessageLogged(object sender, Mtszmj.Log.LogMessageEventArgs e)
        {
            Logs.Add(e.Message);
        }
    }
}
