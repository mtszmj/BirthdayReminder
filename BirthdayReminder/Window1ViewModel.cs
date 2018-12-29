using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BirthdayReminder
{
    class Window1ViewModel : ViewModelBase
    {
        public Window1ViewModel()
        {
            TestInit();
        }

        private void TestInit()
        {
            Person Ania = new Person("Ania Maj", new DateTime(1990, 08, 01), true);
            Person Mateusz = new Person("Mateusz Maj", new DateTime(2018, 10, 23), false);
            Person xxx = new Person("Celcjusz DRdrdw", new DateTime(2018, 6, 23), false);
            DateTime today = DateTime.Today;

            Person Yesterday = new Person("Yesterday Man", new DateTime(1999, today.Month, today.Day - 1), true);
            Person Today = new Person("Today Man", new DateTime(2011, today.Month, today.Day), true);
            Person Tomorrow = new Person("Tomorrow Man", new DateTime(1990, today.Month, today.Day + 1), true);

            People2.Add(Ania);
            People2.Add(Mateusz);
            People2.Add(xxx);
            People2.Add(Yesterday);
            People2.Add(Today);
            People2.Add(Tomorrow);
        }

        public ObservableCollection<Person> People2 { get; set; } = new ObservableCollection<Person>();

        private Person _SelectedPerson;
        public Person SelectedPerson {
            get => _SelectedPerson;
            set => this.SetField(ref _SelectedPerson, value);
        }

        public void AddPerson(Person p)
        {
            var viewModel = new AddEditPersonViewModel();
            var AddEditWindow = new AddEditPersonWindow(viewModel);
            AddEditWindow.ShowDialog();
            if (viewModel.Result)
            {
                People2.Add(viewModel.Person);
            }
            
        }

        public void EditPerson()
        {
            if (_SelectedPerson == null)
                MessageBox.Show("Nie wybrano nikogo do edycji", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                var viewModel = new AddEditPersonViewModel(_SelectedPerson);
                var AddEditWindow = new AddEditPersonWindow(viewModel);
                AddEditWindow.ShowDialog();
                if (viewModel.Result) { 
                    SelectedPerson.Name = viewModel.Person.Name;
                    SelectedPerson.DateOfBirth = viewModel.Person.DateOfBirth;
                    SelectedPerson.IsYearSet = viewModel.Person.IsYearSet;
                }
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

        public void DeletePerson()
        {
            if (_SelectedPerson == null)
                MessageBox.Show("Nie wybrano nikogo do usunięcia", "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                People2.Remove(_SelectedPerson);
            }            
        }

        public void AddSortingByName()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            dataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
            dataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
        }

        public void AddSortingByDate()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
            dataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
            dataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        public void RemoveSorting()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
            dataView.SortDescriptions.Clear();
        }

        public void AddGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription();
            groupDescription.PropertyName = "MonthName";
            dataView.GroupDescriptions.Add(groupDescription);
        }

        public void RemoveGrouping()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
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
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
            dataView.Filter = ShowNext30Days;
        }

        public void RemoveFiltering()
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(People2);
            dataView.Filter = null;
        }
    }
}
