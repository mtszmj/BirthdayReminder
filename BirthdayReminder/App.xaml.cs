using Mtszmj.Logger;
using BirthdayReminder.Model.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using BirthdayReminder.Model.Service.Notifier;
using BirthdayReminder;

namespace BirthdayReminder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public ObservableCollection<Person> People { get; set; } = new ObservableCollection<Person>();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IDataService dataService = new XmlFileDataService(@"C:/Temp/Birth/birth.xml");
            //dataService = new TestDataService();
            //var notifyService = new NotifierBuilder().OfType(NotifierType.Console).Build();

            var notifyIcon = new System.Windows.Forms.NotifyIcon();

            var notifyService = new NotifierBuilder()
                .OfType(NotifierType.Email)
                .SetEmailFrom(BirthdayReminder.Properties.Settings.Default.EmailFrom)
                .SetEmailFromName(BirthdayReminder.Properties.Settings.Default.EmailFromName)
                .AddEmailTo(BirthdayReminder.Properties.Settings.Default.EmailTo)
                .SetSubject(BirthdayReminder.Properties.Settings.Default.Subject)
                .SetPathToPassword(BirthdayReminder.Properties.Settings.Default.PathToPwd)
                .WithSmtp(BirthdayReminder.Properties.Settings.Default.Smtp,
                    BirthdayReminder.Properties.Settings.Default.Port)
                .Disabled()
                .Build();

            notifyService = new NotifierRetryDecorator(notifyService, 3);

            var notifyService2 = new NotifierBuilder()
                .OfType(NotifierType.NotifyIcon)
                .WithNotifyIcon(notifyIcon)
                .WithNotifyTime(1000 * 60 * 6)
                .Enabled()
                .Build();

            notifyService2 = new NotifierRetryDecorator(notifyService2);

            //notifyService = new NotifierBuilder().OfType(NotifierType.Console).Enabled().Build();
            LogViewModel logVM = null;
#if DEBUG
            logVM = new LogViewModel();
#endif

            MainViewModel wvm = new MainViewModel(dataService,
                new[] { notifyService, notifyService2 },
                notifyIcon,
                logVM);
            
        }
    }
}
