using BirthdayReminder.ViewModel;
using Mtszmj.Logger;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace BirthdayReminder
{
    public class LogViewModel : ViewModelBase, ILogViewModel
    {
        public ObservableCollection<LogMessage> Logs { get; set; } = new ObservableCollection<LogMessage>();

        protected override Type _Window => typeof(LogView);

        public LogViewModel()
        {
            Logger.Log.OnMessageLogged += Log_OnMessageLogged;
        }

        private void Log_OnMessageLogged(object sender, LogMessageEventArgs e)
        {
            Logs.Add(e.Message);
        }
    }
}
