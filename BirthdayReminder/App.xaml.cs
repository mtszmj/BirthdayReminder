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
            var notifyService = new NotifierBuilder()
                .OfType(NotifierType.Email)
                .SetEmailFrom(BirthdayReminder.Properties.Settings.Default.EmailFrom)
                .SetEmailFromName(BirthdayReminder.Properties.Settings.Default.EmailFromName)
                .AddEmailTo(BirthdayReminder.Properties.Settings.Default.EmailTo)
                .SetSubject(BirthdayReminder.Properties.Settings.Default.Subject)
                .SetPathToPassword(BirthdayReminder.Properties.Settings.Default.PathToPwd)
                .WithSmtp(BirthdayReminder.Properties.Settings.Default.Smtp,
                    BirthdayReminder.Properties.Settings.Default.Port)
                .Enabled()
                .Build()
                ;

            //notifyService = new NotifierBuilder().OfType(NotifierType.Console).Enabled().Build();

            var logVM = new LogViewModel();

            MainViewModel wvm = new MainViewModel(dataService,
                notifyService, 
                logVM);
            wvm.Show();
        }
    }
}
