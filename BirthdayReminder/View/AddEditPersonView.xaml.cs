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
    /// Interaction logic for AddEditPersonView.xaml
    /// </summary>
    public partial class AddEditPersonView : Window
    {
        public AddEditPersonView(AddEditPersonViewModel viewModel)
        {
            InitializeComponent();
            DataContext = ViewModel = viewModel;
        }

        protected AddEditPersonViewModel ViewModel { get; }

        private void AddPersonButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Result = true;
            Close();
        }
    }
}
