using BirthdayReminder.ViewModel;
using Mtszmj.Logger;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace BirthdayReminder
{
    public class LogViewModel : ViewModelBase, ILogViewModel
    {
        /// <summary>
        /// When adding to ObservableCollection it is needed to do so from UI thread. 
        /// URL: https://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
        /// </summary>
        SynchronizationContext _UiContext;
        public LogViewModel()
        {
            _UiContext = SynchronizationContext.Current;
            Logger.Log.OnMessageLogged += Log_OnMessageLogged;
        }

        public ObservableCollection<LogMessage> Logs { get; set; } = new ObservableCollection<LogMessage>();

        protected override Type _Window => typeof(LogView);

        private void Log_OnMessageLogged(object sender, LogMessageEventArgs e)
        {
            _UiContext.Send(x => Logs.Add(e.Message), null);            
        }
    }
}
