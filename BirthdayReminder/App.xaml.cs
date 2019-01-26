using BirthdayReminder.Model.Service;
using BirthdayReminder.Model.Service.Notifier;
using BirthdayReminder.Model.Service.Password;
using System.Collections.ObjectModel;
using System.Windows;

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
            // Data service for loading and saving contacts data
            IDataService dataService = new XmlFileDataService(
                BirthdayReminder.Properties.Settings.Default.PathToData
                );

            // Handling logging to email (loading encrypted password)
            ILoginHandler loginHandler = new LoginHandler(
                BirthdayReminder.Properties.Settings.Default.PathToPassword,
                BirthdayReminder.Properties.Settings.Default.PathToSalt
                );

            // notify icon in the system
            var notifyIcon = new System.Windows.Forms.NotifyIcon();

            // notify service - email sender
            var notifyService = new NotifierBuilder()
                .OfType(NotifierType.Email)
                .SetEmailFrom(BirthdayReminder.Properties.Settings.Default.EmailFromPath)
                .SetEmailFromName(BirthdayReminder.Properties.Settings.Default.EmailFromName)
                .AddEmailTo(BirthdayReminder.Properties.Settings.Default.EmailToPath)
                .SetSubject(BirthdayReminder.Properties.Settings.Default.Subject)
                .WithSmtp(BirthdayReminder.Properties.Settings.Default.Smtp,
                    BirthdayReminder.Properties.Settings.Default.Port)
                .WithLoginHandler(loginHandler)
                .Enabled()
                .Build();

            // notify decortor - retry before throwing exception
            notifyService = new NotifierRetryDecorator(notifyService, 3);

            // notiify service - notify icon
            var notifyService2 = new NotifierBuilder()
                .OfType(NotifierType.NotifyIcon)
                .WithNotifyIcon(notifyIcon)
                .WithNotifyTime(BirthdayReminder.Properties.Settings.Default.BaloonNotificationTipTime)
                .Enabled()
                .Build();

            // log window - using logger dll I created
            LogViewModel logVM = null;
#if DEBUG
            logVM = new LogViewModel();
#endif

            // main window
            MainViewModel wvm = new MainViewModel(
                dataService,
                new[] { notifyService, notifyService2 },
                notifyIcon,
                logVM);

        }
    }
}
