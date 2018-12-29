using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace BirthdayReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CollectionViewSource listingDataView;

        public MainWindow()
        {
            InitializeComponent();
            listingDataView = (CollectionViewSource)(this.Resources["listingDataView"]);
            AddSortingByDate(this, null);
            
            Person p = ((App)Application.Current).People.First();

            AddPersonWindow add = new AddPersonWindow(p);
            add.Show();
        }

        private void AddSortingByName(object sender, RoutedEventArgs args)
        {
            listingDataView.SortDescriptions.Clear();
            listingDataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            listingDataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
            listingDataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
        }

        private void AddSortingByDate(object sender, RoutedEventArgs args)
        {
            listingDataView.SortDescriptions.Clear();
            listingDataView.SortDescriptions.Add(new SortDescription("Month", ListSortDirection.Ascending));
            listingDataView.SortDescriptions.Add(new SortDescription("Day", ListSortDirection.Ascending));
            listingDataView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        private void RemoveSorting(object sender, RoutedEventArgs args)
        {
            listingDataView.SortDescriptions.Clear();
        }

        private void AddGrouping(object sender, RoutedEventArgs args)
        {
            PropertyGroupDescription groupDescription = new PropertyGroupDescription();
            groupDescription.PropertyName = "MonthName";
            listingDataView.GroupDescriptions.Add(groupDescription);
        }

        private void RemoveGrouping(object sender, RoutedEventArgs args)
        {
            listingDataView.GroupDescriptions.Clear();
        }

        private void ShowOnlyThisYear(object sender, FilterEventArgs args)
        {
            Person person = args.Item as Person;
            if (person != null)
            {
                DateTime today = DateTime.Today;
                if (person.Month > today.Month
                    || (person.Month == today.Month && person.Day >= today.Day))
                {
                    args.Accepted = true;
                }
                else
                {
                    args.Accepted = false;
                }
            }
        }

        private void AddFiltering(object sender, RoutedEventArgs args)
        {
            listingDataView.Filter += new FilterEventHandler(ShowOnlyThisYear);
        }


        private void RemoveFiltering(object sender, RoutedEventArgs args)
        {
            listingDataView.Filter -= new FilterEventHandler(ShowOnlyThisYear);
        }
    }
}
