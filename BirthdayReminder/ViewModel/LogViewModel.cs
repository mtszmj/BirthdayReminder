using BirthdayReminder.ViewModel;
using Mtszmj.Logger;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace BirthdayReminder
{
    public class LogViewModel : ViewModelBase, ILogViewModel
    {
        public LogViewModel()
        {
            Logger.Log.OnMessageLogged += Log_OnMessageLogged;
        }

        public ObservableCollection<LogMessage> Logs { get; set; } = new ObservableCollection<LogMessage>();

        protected override Type _Window => typeof(LogView);

        private void Log_OnMessageLogged(object sender, LogMessageEventArgs e)
        {
            Logs.Add(e.Message);
        }
    }
}
