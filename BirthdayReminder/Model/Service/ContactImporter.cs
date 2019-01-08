using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BirthdayReminder.Model.Service
{
    public class ContactImporter
    {
        public IEnumerable<Person> Import(string path)
        {
            if(path == null || !File.Exists(path))
            {
                throw new ArgumentException(nameof(path));
            }

            List<Person> people = new List<Person>();
            using (var reader = new StreamReader(path))
            {
                var firstLine = reader.ReadLine();
                var format = new GoogleFormat();
                
                if (format.IsGoogleFormat(firstLine))
                {
                    string line;
                    while((line = reader.ReadLine()) != null)
                    {
                        if (format.TryConvert(line, out Person person))
                            people.Add(person);
                    }
                }

                else
                {
                    Logger.Log.LogError("Niepoprawny format pliku csv");
                }
            }

            return people;
        }

        private class GoogleFormat
        {
            private const char SPLITTER = ',';
            private const int LENGTH_AFTER_SPLIT = 65;
            private const string NAME = "Name";
            private const string BIRTHDAY = "Birthday";
            private const char BIRTHDAY_SPLITTER = '-';
            private const int PARTS_WITH_YEAR_SET = 3;
            private const int PARTS_WITHOUT_YEAR_SET = 4;
            private static Dictionary<string, int> ContactData { get; } = new Dictionary<string, int>()
            {
                [NAME] = 0,
                [BIRTHDAY] = 14,
            };

            public bool IsGoogleFormat(string firstLine)
            {
                var data = firstLine.Split(SPLITTER);
                if (data.Length != LENGTH_AFTER_SPLIT)
                    return false;
                if (data[ContactData[NAME]] == NAME && data[ContactData[BIRTHDAY]] == BIRTHDAY)
                    return true;

                return false;
            }

            public bool TryConvert(string line, out Person person)
            {
                person = null;
                var data = line.Split(SPLITTER);
                if (data == null || data.Length != LENGTH_AFTER_SPLIT)
                    return false;
                var birthday = data[ContactData[BIRTHDAY]];

                if (birthday != string.Empty)
                {
                    var (date, isYearSet) = StringToDateTime(birthday);
                    person = Person.Factory.CreatePerson(data[ContactData[NAME]], date, isYearSet);
                    Logger.Log.LogInfo($"{data[ContactData[NAME]]} - {date} - {isYearSet}");
                    return true;
                }
                else return false;
            }

            private (DateTime date, bool isYearSet) StringToDateTime(string birthday)
            {
                var parts = birthday.Split(BIRTHDAY_SPLITTER);

                if (parts.Length == PARTS_WITH_YEAR_SET)
                    return (new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])), true);
                else if (parts.Length == PARTS_WITHOUT_YEAR_SET)
                    return (new DateTime(2000, int.Parse(parts[2]), int.Parse(parts[3])), false);
                else
                    return (default(DateTime), false);
            }



            // Google CSV format:
            //Name,Given Name,Additional Name,Family Name,Yomi Name,Given Name Yomi,Additional Name Yomi,
            //Family Name Yomi,Name Prefix,Name Suffix,Initials,Nickname,Short Name,Maiden Name,Birthday,
            //Gender,Location,Billing Information,Directory Server,Mileage,Occupation,Hobby,Sensitivity,
            //Priority,Subject,Notes,Language,Photo,Group Membership,E-mail 1 - Type,E-mail 1 - Value,
            //E-mail 2 - Type,E-mail 2 - Value,Phone 1 - Type,Phone 1 - Value,Phone 2 - Type,Phone 2 - Value,
            //Address 1 - Type,Address 1 - Formatted,Address 1 - Street,Address 1 - City,Address 1 - PO Box,
            //Address 1 - Region,Address 1 - Postal Code,Address 1 - Country,Address 1 - Extended Address,
            //Address 2 - Type,Address 2 - Formatted,Address 2 - Street,Address 2 - City,Address 2 - PO Box,
            //Address 2 - Region,Address 2 - Postal Code,Address 2 - Country,Address 2 - Extended Address,
            //Organization 1 - Type,Organization 1 - Name,Organization 1 - Yomi Name,Organization 1 - Title,
            //Organization 1 - Department,Organization 1 - Symbol,Organization 1 - Location,
            //Organization 1 - Job Description,Event 1 - Type,Event 1 - Value
        }


    }
}
