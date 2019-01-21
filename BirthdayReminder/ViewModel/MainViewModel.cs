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
        private RelayCommand _ImportCommand;
        private RelayCommand _OpenLogWindow;
        private RelayCommand _RemovePersonCommand;
        private readonly object IsImportStartedLocker = new object();
        private readonly System.Windows.Threading.DispatcherTimer NotifyTimer;

        public MainViewModel(IDataService dataService, INotifyService notifyService, ILogViewModel logVM = null)
        {
            Dispatcher.CurrentDispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            AddSortingByDate();
            LoadSettings();
            DataService = dataService;
            NotifyService = notifyService;
            _LogViewModel = logVM;
            LoadData();

            LastNotify = DateTime.Today.AddDays(-1); // TODO usunac

            Logger.Log.LogDebug(LastNotify.ToString());
            NotifyTimer = new DispatcherTimer();
            NotifyTimer.Interval = TimeSpan.FromSeconds(5);
            NotifyTimer.Tick += NotifyTimer_Tick;
            NotifyTimer.Start();
        }

        private async void NotifyTimer_Tick(object sender, EventArgs e)
        {
            await NotifyAsync();
        }

        private async Task NotifyAsync()
        {
            var date = LastNotify;
            var now = DateTime.Now;

            if (NotifyService.Enabled && date.Day != now.Day)
            {
                var todaysBirthdays = PeopleCollection.Where(p => p.DaysToBirthday == 0);
                if (todaysBirthdays.Any() || true)
                {
                    try
                    {
                        Logger.Log.LogDebug("Trying to send.");
                        await Task.Run(() =>
                            NotifyService?.Notify(
                                todaysBirthdays,
                                PeopleCollection.Where(p =>
                                    p.DaysToBirthday < Properties.Settings.Default.DaysForwardInNotify
                                    && p.DaysToBirthday > 0)
                                )
                        );
                        Logger.Log.LogDebug("Sent correctly");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"Wystąpił błąd. Sprawdź konfigurację usługi powiadomień (ustawienia).\n{exception.ToString()}");
                        Logger.Log.LogError("Bład wysyłania");
                        Logger.Log.LogError(exception.ToString());
                    }
                }
                LastNotify = DateTime.Now;
                Logger.Log.LogError("Sprawdzone czy ktoś ma dzisiaj urodziny, zaktualizowana data ostatniego powiadomienia.");
                Logger.Log.LogDebug(LastNotify.ToString());
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

        public RelayCommand ExitCommand { get; }
            = new RelayCommand(o => Application.Current.Shutdown());

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

        private INotifyService NotifyService { get; set; }

        private ILogViewModel _LogViewModel { get; }


        private void AddGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription();
            groupDescription.PropertyName = "MonthName";
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
                dataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }

        private void AddSortingByName()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            using (dataView.DeferRefresh()) // use the DeferRefresh so that we refresh only once
            {

                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
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
                MessageBox.Show("Nie wybrano nikogo do edycji", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
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
                Status = "Importuję kontakty...";
                result = await Task.Run(() => csv.Import());
                IsImportStarted = false;
                Status = "Zaimportowano";

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
                MessageBox.Show("Nie wybrano nikogo do usunięcia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
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

    }
}
