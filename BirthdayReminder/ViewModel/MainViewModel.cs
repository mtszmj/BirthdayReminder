﻿using BirthdayReminder.Model.Service;
using BirthdayReminder.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace BirthdayReminder
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IDataService dataService, ILogViewModel logVM = null)
        {
            DataService = dataService;
            _LogViewModel = logVM;
            OpenLogWindow = new RelayCommand(o => _LogViewModel?.Show(),
                               o => (_LogViewModel != null)
                               );
            LoadData();
        }

        private IDataService DataService { get; set; }
        private ILogViewModel _LogViewModel { get; }
        public ObservableCollection<Person> PeopleCollection { get; set; } = new ObservableCollection<Person>();
        
        private Person _SelectedPerson;
        public Person SelectedPerson
        {
            get => _SelectedPerson;
            set => this.SetField(ref _SelectedPerson, value);
        }

        public RelayCommand ExitCommand { get; }
            = new RelayCommand(o => Application.Current.Shutdown());
        public RelayCommand OpenLogWindow { get; }

        //public RelayCommand ImportCommand { get; }
        //    = new RelayCommand(o => null);

        protected override Type _Window => typeof(MainView);

        

        private void LoadData()
        {
            foreach (var person in DataService?.GetPeople())
            {
                PeopleCollection.Add(person);
            }
        }

        private void SaveData()
        {
            DataService.SavePeople(PeopleCollection);
        }

        public void AddPerson(Person p)
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

        public void EditPerson()
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

        public void DeletePerson()
        {
            if (_SelectedPerson == null)
                MessageBox.Show("Nie wybrano nikogo do usunięcia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                PeopleCollection.Remove(_SelectedPerson);
            }
        }

        public void AddSortingByName()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            using (dataView.DeferRefresh()) // we use the DeferRefresh so that we refresh only once
            {

                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
            }
        }

        public void AddSortingByDate()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            using (dataView.DeferRefresh()) // we use the DeferRefresh so that we refresh only once
            {
                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
                dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }

        public void RemoveSorting()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.SortDescriptions.Clear();
        }

        public void AddGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription();
            groupDescription.PropertyName = "MonthName";
            dataView.GroupDescriptions.Add(groupDescription);
        }

        public void RemoveGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.GroupDescriptions.Clear();
        }

        protected bool ShowOnlyThisYear(object sender)
        {
            Person person = sender as Person;
            if (person != null)
            {
                DateTime today = DateTime.Today;
                if (person.Month > today.Month
                    || (person.Month == today.Month && person.Day >= today.Day))
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ShowNext30Days(object sender)
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

        public void AddFiltering30Days()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.Filter = ShowNext30Days;
        }

        public void RemoveFiltering()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(PeopleCollection);
            dataView.Filter = null;
        }
    }
}