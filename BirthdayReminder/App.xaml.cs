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
            dataService = new TestDataService();
            var notifyService = new ConsoleNotifyService();
            var logVM = new LogViewModel();

            MainViewModel wvm = new MainViewModel(dataService, logVM);
            wvm.Show();
            

            notifyService.Notify();
        }
    }
}
