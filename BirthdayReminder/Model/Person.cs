using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BirthdayReminder
{
    [Serializable]
    public class Person : INotifyPropertyChanged, IEquatable<Person>
    {
        public const int DEFAULT_YEAR = 2000;

        protected DateTime _DateOfBirth;
        protected bool _IsYearSet;
        protected string _Name;

        protected Person()
        {
            IsYearSet = true;
            DateOfBirth = DateTime.Today;
        }

        protected Person(string name, DateTime dateOfBirth, bool isYearSet)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            IsYearSet = isYearSet;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region IEquatable

        public override bool Equals(object obj)
        {
            return Equals(obj as Person);
        }

        public bool Equals(Person other)
        {
            return other != null &&
                   Name == other.Name &&
                   DateOfBirth == other.DateOfBirth &&
                   IsYearSet == other.IsYearSet;
        }

        public static bool operator ==(Person person1, Person person2)
        {
            if (ReferenceEquals(person1, null))
            {
                return ReferenceEquals(person2, null);
            }
            return (person1.Equals(person2));
        }

        public static bool operator !=(Person person1, Person person2)
        {
            return !(person1 == person2);
        }

        // GetHashCode is not implemented - object is mutable and overriden GetHashCode generates Exceptions.
        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        var result = 13;
        //        result = (result * 397) ^ Name.GetHashCode();
        //        result = (result * 397) ^ DateOfBirth.GetHashCode();
        //        result = (result * 397) ^ IsYearSet.GetHashCode();
        //        return result;
        //    }
        //}

        #endregion

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.AddYears(age) > today)
                    age--;
                return age;
            }

        }

        public string Birthday
        {
            get
            {
                if (IsYearSet)
                    return DateOfBirth.ToString("dd.MM.yyyy");
                else
                    return DateOfBirth.ToString("dd.MM");
            }
        }

        public DateTime DateOfBirth
        {
            get => _DateOfBirth;
            set => SetField(ref _DateOfBirth, value);
        }

        public int Day => DateOfBirth.Day;

        public int DaysToBirthday
        {
            get
            {
                var today = DateTime.Today;
                var dateOfBirth = new DateTime(today.Year, _DateOfBirth.Month, _DateOfBirth.Day);
                if (dateOfBirth < today)
                    dateOfBirth = dateOfBirth.AddYears(1);
                var timeSpan = dateOfBirth - today;
                return timeSpan.Days;
            }
        }

        public bool IsYearSet
        {
            get => _IsYearSet;
            set => SetField(ref _IsYearSet, value);
        }

        public int Month => DateOfBirth.Month;

        public string MonthName => DateOfBirth.ToString("MMMM");

        public string Name
        {
            get => _Name;
            set => SetField(ref _Name, value);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Name}, ");
            sb.Append(IsYearSet ? DateOfBirth.ToString("dd.MM.yyyy") : DateOfBirth.ToString("dd.MM"));
            return sb.ToString();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #region Factory

        public class Factory
        {
            public static Person CreateEmptyPerson()
            {
                return new Person();
            }
            public static Person CreatePerson(string name, DateTime dateOfBirth, bool isYearSet)
            {
                if (!isYearSet && dateOfBirth.Year != DEFAULT_YEAR)
                    dateOfBirth = new DateTime(DEFAULT_YEAR, dateOfBirth.Month, dateOfBirth.Day);
                return new Person(name, dateOfBirth, isYearSet);
            }
            public static Person DeepCopy(Person person)
            {
                return new Person(person.Name, person.DateOfBirth, person.IsYearSet);
            }
        }

        #endregion
    }
}
