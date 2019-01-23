using BirthdayReminder.Model.Service;
using BirthdayReminder.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace BirthdayReminder
{
    public class MainViewModel : ViewModelBase
    {
        private Person _SelectedPerson;
        private string _Status;
        private bool _IsFiltered;
        private bool _IsGrouped;
        private bool _IsImportStarted;
        private bool _IsSortedByName;
        private RelayCommand _AddPersonCommand;
        private RelayCommand _EditPersonCommand;
        private RelayCommand _ExitCommand;
        private RelayCommand _MinimizeCommand;
        private RelayCommand _ImportCommand;
        private RelayCommand _OpenLogWindow;
        private RelayCommand _RemovePersonCommand;
        private RelayCommand _StartWithSystem;
        private readonly object IsImportStartedLocker = new object();
        private readonly System.Windows.Threading.DispatcherTimer NotifyTimer;
        private System.Windows.Forms.NotifyIcon _NotifyIcon;


        public MainViewModel(IDataService dataService, IEnumerable<INotifyService> notifyService, System.Windows.Forms.NotifyIcon notifyIcon, ILogViewModel logVM = null)
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

            LoadSettings();
            SetNotifyIcon(notifyIcon);

            DataService = dataService;
            NotifyService.AddRange(notifyService);
            _LogViewModel = logVM;

            var isAutoStart = GetStartWithSystem();
            ((MainView)View).StartWithSystemCheckbox.IsChecked = isAutoStart;
            AddSortingByDate();

            LoadData();
            View.Closing += View_Closing;

            Logger.Log.LogDebug(LastNotify.ToString());

            NotifyTimer = new DispatcherTimer();
            NotifyTimer.Interval = Properties.Settings.Default.BaloonNotificationTipTime;
            NotifyTimer.Tick += NotifyTimer_Tick;
            NotifyTimer.Start();

            if (isAutoStart)
            {
                View.Hide();
                _NotifyIcon.ShowBalloonTip(Properties.Settings.Default.BaloonBasicTipTime, 
                    Properties.Settings.Default.AppName, 
                    Properties.Resources.Started, 
                    System.Windows.Forms.ToolTipIcon.None);
            }
            else
            {
                View.Show();
            }
        }



        public ObservableCollection<Person> PeopleCollection { get; set; } = new ObservableCollection<Person>();

        public Person SelectedPerson
        {
            get => _SelectedPerson;
            set
            {
                SetField(ref _SelectedPerson, value);
                _EditPersonCommand?.RaiseCanExecuteChanged();
                _RemovePersonCommand?.RaiseCanExecuteChanged();
            }
        }

        public string Status
        {
            get => _Status;
            set
            {
                SetField(ref _Status, value);
            }
        }

        public bool IsAutoStart => GetStartWithSystem();

        public Visibility IsDebug
        {
#if DEBUG
            get => Visibility.Visible;
#else
            get => Visibility.Collapsed;
#endif
        }

        public bool IsFiltered
        {
            get => _IsFiltered;
            set
            {
                if (_IsFiltered == value)
                    return;
                _IsFiltered = value;
                if (_IsFiltered)
                    AddFiltering30Days();
                else
                    RemoveFiltering();
                OnPropertyChanged(nameof(IsFiltered));
            }
        }

        public bool IsGrouped
        {
            get => _IsGrouped;
            set
            {
                if (_IsGrouped == value)
                    return;
                _IsGrouped = value;
                if (_IsGrouped)
                    AddGrouping();
                else
                    RemoveGrouping();
                OnPropertyChanged(nameof(IsGrouped));
            }
        }

        public bool IsSortedByName
        {
            get => _IsSortedByName;
            set
            {
                if (_IsSortedByName == value)
                    return;
                _IsSortedByName = value;
                if (_IsSortedByName)
                    AddSortingByName();
                else
                    AddSortingByDate();
                OnPropertyChanged(nameof(IsSortedByName));
            }
        }

        public DateTime LastNotify
        {
            get
            {
                return Properties.Settings.Default.LastNotify;
            }
            set
            {
                Properties.Settings.Default.LastNotify = value;
            }
        }

        public RelayCommand AddPersonCommand
        {
            get
            {
                if (_AddPersonCommand == null)
                {
                    _AddPersonCommand = new RelayCommand(o => AddPerson(),
                        o => !IsImportStarted);
                }
                return _AddPersonCommand;
            }
        }

        public RelayCommand EditPersonCommand
        {
            get
            {
                if (_EditPersonCommand == null)
                {
                    _EditPersonCommand = new RelayCommand(o => EditPerson(),
                        o => _SelectedPerson != null && !IsImportStarted);
                }
                return _EditPersonCommand;
            }
        }

        public RelayCommand ExitCommand
        {
            get
            {
                if (_ExitCommand == null)
                {
                    _ExitCommand = new RelayCommand(o => ShutdownAction());
                }
                return _ExitCommand;
            }
        }

        public RelayCommand ImportCommand
        {
            get
            {
                if (_ImportCommand == null)
                {
                    _ImportCommand = new RelayCommand(o => ImportAction(),
                        o => ImportPredicate()
                        );
                }
                return _ImportCommand;
            }
        }

        public RelayCommand MinimizeCommand
        {
            get
            {
                if (_MinimizeCommand == null)
                {
                    _MinimizeCommand = new RelayCommand(o => MinimizeAction(o as CancelEventArgs));
                }
                return _MinimizeCommand;
            }
        }

        public RelayCommand OpenLogWindow
        {
            get
            {
                if (_OpenLogWindow == null)
                {
                    _OpenLogWindow = new RelayCommand(o => _LogViewModel?.Show(),
                               o => (_LogViewModel != null)
                               );
                }
                return _OpenLogWindow;
            }
        }

        public RelayCommand RemovePersonCommand
        {
            get
            {
                if (_RemovePersonCommand == null)
                {
                    _RemovePersonCommand = new RelayCommand(o => RemovePerson(),
                        o => _SelectedPerson != null && !IsImportStarted);
                }
                return _RemovePersonCommand;
            }
        }

        public RelayCommand StartWithSystem
        {
            get
            {
                if (_StartWithSystem == null)
                {
                    _StartWithSystem = new RelayCommand(o => StartWithSystemAction((bool)o));
                }
                return _StartWithSystem;
            }
        }

        protected override Type _Window => typeof(MainView);

        private bool IsImportStarted
        {
            get => _IsImportStarted;
            set
            {
                if (_IsImportStarted == value)
                    return;
                _IsImportStarted = value;
                ImportCommand?.RaiseCanExecuteChanged();
                AddPersonCommand?.RaiseCanExecuteChanged();
                EditPersonCommand?.RaiseCanExecuteChanged();
                RemovePersonCommand?.RaiseCanExecuteChanged();
            }
        }

        private IDataService DataService { get; set; }

        private List<INotifyService> NotifyService { get; } = new List<INotifyService>();

        private ILogViewModel _LogViewModel { get; }


        private void AddGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription();
            groupDescription.PropertyName = nameof(Person.MonthName);
            dataView.GroupDescriptions.Add(groupDescription);
        }

        private void AddFiltering30Days()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.Filter = ShowNext30Days;
        }

        private void AddPerson()
        {
            var viewModel = new AddEditPersonViewModel();
            var AddEditWindow = new AddEditPersonView(viewModel);
            AddEditWindow.ShowDialog();
            if (viewModel.Result)
            {
                PeopleCollection.Add(viewModel.Person);
                SaveData();
            }
        }

        private void AddSortingByDate()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            using (dataView.DeferRefresh()) // use the DeferRefresh so that we refresh only once
            {
                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription(nameof(Person.Month), ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription(nameof(Person.Day), ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription(nameof(Person.Name), ListSortDirection.Ascending));
            }
        }

        private void AddSortingByName()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            using (dataView.DeferRefresh()) // use the DeferRefresh so that we refresh only once
            {

                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription(nameof(Person.Name), ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription(nameof(Person.Month), ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription(nameof(Person.Day), ListSortDirection.Ascending));
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            NotifyTimer?.Stop();
            SaveData();
            SaveSettings();
        }

        private void EditPerson()
        {
            if (_SelectedPerson == null)
                MessageBox.Show(Properties.Resources.NobodySelectedForEdition, 
                    Properties.Resources.Error, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            else
            {
                var viewModel = new AddEditPersonViewModel(_SelectedPerson);
                var AddEditWindow = new AddEditPersonView(viewModel);
                AddEditWindow.ShowDialog();
                if (viewModel.Result)
                {
                    SelectedPerson.Name = viewModel.Person.Name;
                    SelectedPerson.DateOfBirth = viewModel.Person.DateOfBirth;
                    SelectedPerson.IsYearSet = viewModel.Person.IsYearSet;
                    SaveData();
                    OnPropertyChanged(nameof(SelectedPerson));
                }
            }
        }

        private bool GetStartWithSystem()
        {
            var result = false;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Properties.Settings.Default.RegistryKey, true))
            {
                result = key.GetValue(Properties.Settings.Default.AppName) != null;
            }

            return result;
        }

        private void LoadData()
        {
            foreach (var person in DataService?.GetPeople())
            {
                PeopleCollection.Add(person);
            }
        }

        private void LoadSettings()
        {
            IsSortedByName = Properties.Settings.Default.IsSortedByName;
            IsGrouped = Properties.Settings.Default.IsGrouped;
            IsFiltered = Properties.Settings.Default.IsFiltered;
        }

        private async void ImportAction()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Contact files (csv, vcf)|*.csv;*vcf";
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == true)
            {
                IEnumerable<Person> result = null;

                ContactImporter csv = ContactImporter.Factory.CreateFor(ofd.FileName);
                IsImportStarted = true;
                Status = Properties.Resources.ContactAreImported;
                result = await Task.Run(() => csv.Import());
                IsImportStarted = false;
                Status = Properties.Resources.ImportFinished;

                foreach (var person in result)
                {
                    if (!PeopleCollection.Contains(person))
                    {
                        PeopleCollection.Add(person);
                    }
                }

                await Task.Run(() => { Thread.Sleep(1500); Status = ""; });
            }
        }

        private bool ImportPredicate()

        {
            return !IsImportStarted;
        }

        private void MinimizeAction(CancelEventArgs args)
        {
            if (args != null)
                args.Cancel = true;
            Hide();
            _NotifyIcon?.ShowBalloonTip(Properties.Settings.Default.BaloonBasicTipTime, 
                Properties.Settings.Default.AppName, 
                Properties.Resources.MinimizeToTray, 
                System.Windows.Forms.ToolTipIcon.None);
        }

        private async void NotifyTimer_Tick(object sender, EventArgs e)
        {
            await Notify();
        }

        private async Task Notify()
        {
            var date = LastNotify;
            var now = DateTime.Now;

            foreach(var notifyService in NotifyService)
            {
                if (notifyService.Enabled && date.Day != now.Day)
                {
                    var todaysBirthdays = PeopleCollection.Where(p => p.DaysToBirthday == 0);
                    if (todaysBirthdays.Any())
                    {
                        try
                        {
                            await Task.Run(() =>
                                notifyService?.Notify(
                                    todaysBirthdays,
                                    PeopleCollection.Where(p =>
                                        p.DaysToBirthday < Properties.Settings.Default.DaysForwardInNotify
                                        && p.DaysToBirthday > 0)
                                    )
                            );

                            Logger.Log.LogDebug($"Sent correctly for {notifyService.GetType().Name}");
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show($"{Properties.Resources.EmailException}{Environment.NewLine}{exception.ToString()}");
                            Logger.Log.LogError("Bład wysyłania");
                            Logger.Log.LogError(exception.ToString());
                        }
                    }
                    LastNotify = DateTime.Now;
                    Logger.Log.LogInfo("Sprawdzone czy ktoś ma dzisiaj urodziny, zaktualizowana data ostatniego powiadomienia.");
                    Logger.Log.LogDebug(LastNotify.ToString());
                }
            }
        }

        private void RemoveFiltering()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.Filter = null;
        }

        private void RemoveGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.GroupDescriptions.Clear();
        }

        private void RemovePerson()
        {
            if (_SelectedPerson == null)
                MessageBox.Show(Properties.Resources.NobodySelectedForDeletion, 
                    Properties.Resources.Error, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
            else
            {
                PeopleCollection.Remove(_SelectedPerson);
            }
        }

        private void RemoveSorting()

        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.SortDescriptions.Clear();
        }

        private void RemoveStartWithSystem()
        {
            Logger.Log.LogInfo(nameof(RemoveStartWithSystem));
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Properties.Settings.Default.RegistryKey, true))
            {
                key.DeleteValue(Properties.Settings.Default.AppName, false);
            }
            _NotifyIcon.ShowBalloonTip(Properties.Settings.Default.BaloonBasicTipTime, Properties.Resources.AutoStart, Properties.Resources.StartReset, System.Windows.Forms.ToolTipIcon.None);
        }

        private void SaveData()
        {
            DataService?.SavePeople(PeopleCollection);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.IsSortedByName = IsSortedByName;
            Properties.Settings.Default.IsGrouped = IsGrouped;
            Properties.Settings.Default.IsFiltered = IsFiltered;
            Properties.Settings.Default.Save();
        }

        private void SetNotifyContextMenu()

        {
            var cms = _NotifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            cms.Items.Add(Properties.Resources.ShowMainWindow).Click += (s, args) => Show();
            cms.Items.Add(Properties.Resources.Close).Click += (s, args) => ExitCommand.Execute(null);
        }

        private void SetNotifyIcon(System.Windows.Forms.NotifyIcon notifyIcon)
        {
            _NotifyIcon = notifyIcon ?? new System.Windows.Forms.NotifyIcon();
            _NotifyIcon.DoubleClick += (s, args) => Show();
            _NotifyIcon.Icon = Properties.Resources.MainIcon;
            _NotifyIcon.Visible = true;
            _NotifyIcon.Text = Properties.Settings.Default.AppName;

            SetNotifyContextMenu();
        }

        private bool ShowNext30Days(object sender)
        {
            Person person = sender as Person;
            if (person != null)
            {
                if (person.DaysToBirthday <= 30)
                {
                    return true;
                }
            }
            return false;
        }

        private void ShutdownAction()
        {
            View.Closing -= View_Closing;
            _NotifyIcon?.Dispose();
            _NotifyIcon = null;
            Application.Current.Shutdown();
        }

        private void StartWithSystemAction(bool isChecked)
        {
            if (isChecked)
                SetStartWithSystem();
            else
                RemoveStartWithSystem();
        }

        private void SetStartWithSystem()
        {
            Logger.Log.LogInfo(nameof(SetStartWithSystem));
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Properties.Settings.Default.RegistryKey, true))
            {
                key.SetValue(Properties.Settings.Default.AppName, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            _NotifyIcon.ShowBalloonTip(Properties.Settings.Default.BaloonBasicTipTime, Properties.Resources.AutoStart, Properties.Resources.StartSet, System.Windows.Forms.ToolTipIcon.None);
        }

        private void View_Closing(object sender, CancelEventArgs e)
        {
            MinimizeCommand.Execute(e);
        }
    }
}
