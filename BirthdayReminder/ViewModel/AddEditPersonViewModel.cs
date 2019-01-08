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
            IsAdd = true;
        }

        public AddEditPersonViewModel(Person person)
        {
            Person = Person.Factory.DeepCopy(person);
            ButtonText = "Modyfikuj";
            IsAdd = false;
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
        //TODO zmienic button content binding na settery z IsAdd / IsEdit
        public bool IsAdd { get; set; }
        
        public bool Result { get; set; }

        protected override Type _Window => typeof(AddEditPersonView);
    }
}
