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
using System.Windows.Shapes;

namespace BirthdayReminder
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            DataContext = ViewModel = new Window1ViewModel();

        }

        private Window1ViewModel ViewModel;

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddPerson(new Person("test", DateTime.Today, true));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditPerson();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeletePerson();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddSortingByName();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.AddSortingByDate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveSorting();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ViewModel.AddGrouping();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveGrouping();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ViewModel.AddFiltering30Days();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveFiltering();
        }
    }
}
