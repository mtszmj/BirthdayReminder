using BirthdayReminder.ViewModel;
using Mtszmj.Log;
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

        private void Log_OnMessageLogged(object sender, Mtszmj.Log.LogMessageEventArgs e)
        {
            Logs.Add(e.Message);
        }

        public void Show()
        {
            foreach(Window window in Application.Current.Windows)
            {
                if(window is LogView)
                {
                    window.Activate();
                    return;
                }
            }
            new LogView(this).Show();
        }
    }
}
