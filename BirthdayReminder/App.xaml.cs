using Mtszmj.Log;
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
            //InitTestData();
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();

            var notifyService = new ConsoleNotifyService();
            notifyService.Notify();

            var dataService = new XmlFileDataService(@"C:/Temp/Birth/birth.xml");
            Window1ViewModel wvm = new Window1ViewModel(dataService);
            Window1 w = new Window1(wvm);
            w.Show();

            new LogView().Show();
            for (var i = 0; i < 10; i++) { 
                Logger.Log.LogTrace("Test View Modelu");
                Logger.Log.LogDebug("Test View Modelu");
                Logger.Log.LogInfo("Test View Modelu");
                Logger.Log.LogWarning("Test View Modelu");
                Logger.Log.LogError("Test View Modelu");
                Logger.Log.LogCritical("Test View Modelu");
            }
        }

        private void InitTestData()
        {
            Person Ania = new Person("Ania Maj", new DateTime(1990, 08, 01), true);
            Person Mateusz = new Person("Mateusz Maj", new DateTime(2018, 10, 23), false);
            Person xxx = new Person("Celcjusz DRdrdw", new DateTime(2018, 6, 23), false);
            DateTime today = DateTime.Today;

            Person Yesterday = new Person("Yesterday Man", new DateTime(1999, today.Month, today.Day - 1), true);
            Person Today = new Person("Today Man", new DateTime(2011, today.Month, today.Day), true);
            Person Tomorrow = new Person("Tomorrow Man", new DateTime(1990, today.Month, today.Day + 1), true);

            People.Add(Ania);
            People.Add(Mateusz);
            People.Add(xxx);
            People.Add(Yesterday);
            People.Add(Today);
            People.Add(Tomorrow);
        }
    }
}
