using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder
{
    public class AddEditPersonViewModel : ViewModelBase
    {
        private string _ButtonText;
        private Person _ModifiedPerson;

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

        public string ButtonText
        {
            get => _ButtonText;
            set => _ButtonText = value;
        }

        public Person Person
        {
            get => _ModifiedPerson;
            set => SetField(ref _ModifiedPerson, value);
        }

        protected override Type _Window => typeof(AddEditPersonView);

        //TODO zmienic button content binding na settery z IsAdd / IsEdit
        public bool IsAdd { get; set; }
        
        public bool Result { get; set; }
    }
}
