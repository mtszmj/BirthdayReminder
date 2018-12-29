using System;
using System.Windows;

namespace BirthdayReminder
{
    /// <summary>
    /// Interaction logic for AddPersonWindow.xaml
    /// </summary>
    public partial class AddPersonWindow : Window
    {
        private bool EditPerson { get; set; } = false;

        public AddPersonWindow(Person person = null)
        {
            InitializeComponent();
            if (person != null)
            {
                this.DataContext = person;
                this.AddPersonButton.Visibility = Visibility.Hidden;
                EditPerson = true;
            }
            else
            { 
                this.DataContext = new Person(String.Empty, DateTime.Today, true);
            }
        }

        private void OnInit(object sender, RoutedEventArgs e)
        {
            this.DataContext = new Person(String.Empty, DateTime.Today, true);
        }

        private void AddPerson(object sender, RoutedEventArgs args)
        {
            Person person = (Person)(this.DataContext);
            if (person?.Name != string.Empty)
            {
                if(EditPerson) { 
                    ((App)Application.Current).People.Add(person);
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Uzupełnij dane.");
            }
        }
    }
}
