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
    /// Interaction logic for AddEditPersonWindow.xaml
    /// </summary>
    public partial class AddEditPersonView : Window
    {
        protected AddEditPersonViewModel ViewModel;

        public AddEditPersonView(AddEditPersonViewModel viewModel)
        {
            InitializeComponent();
            DataContext = ViewModel = viewModel;
        }

        private void AddPersonButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Result = true;
            Close();
        }
    }
}
