using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder
{
    public class AddEditPersonViewModel : ViewModelBase
    {

        public AddEditPersonViewModel()
        {
            Person = Person.Factory.CreateEmptyPerson();
            ButtonText = "Dodaj";
        }

        public AddEditPersonViewModel(Person person)
        {
            Person = Person.Factory.DeepCopy(person);
            ButtonText = "Modyfikuj";
        }

        Person _ModifiedPerson;
        public Person Person
        {
            get => _ModifiedPerson;
            set => SetField(ref _ModifiedPerson, value);
        }

        public string _ButtonText;
        public string ButtonText
        {
            get => _ButtonText;
            set => _ButtonText = value;
        }

        public bool Result { get; set; }

    }
}
