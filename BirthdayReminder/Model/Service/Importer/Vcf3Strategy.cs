using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service.Importer
{
    internal class Vcf3Strategy : IImportStrategy, IVcfImportStrategy
    {
        private const string BEGIN = "BEGIN:VCARD";
        private const string VERSION = "VERSION:3.0";
        private const string END = "END:VCARD";
        private const string NAME = "name";
        private const string BIRTHDAY = "birthday";
        private const string NAME_REGEX = @"(?<=\r\n\bFN:)(\w|.[^\r\n])+\b"; // look ahead for newline and 'FN:', take chars and whitespaces but no newlines
        private const string BIRTHDAY_REGEX = @"(?<=\r\n\bBDAY:)([\d-]{6,8})\b"; // look ahead for newline and 'BDAY:', take digits or '-' if between 6 and 8 times.
        private static string NAME_BIRTHDAY_REGEX = $@"(?<=\r\n\bFN:)(?<{NAME}>(\w|.[^\r\n])+)\b.*(?<=\r\n\bBDAY:)(?<{BIRTHDAY}>[\d-]{{6,8}})\b"; // sum of the two upper

        private static Lazy<Regex> NameBirthdayRegex = new Lazy<Regex>(() => new Regex(NAME_BIRTHDAY_REGEX, RegexOptions.Singleline | RegexOptions.Compiled));

        public bool IsCorrectFormat(string pathToImportedFile)
        {
            using (var reader = new StreamReader(pathToImportedFile))
            {
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null && line != END)
                {
                    lines.Add(line);
                }
                if (lines.Count(x => x.Contains(BEGIN)) == 1
                    && lines.Find(x => x == VERSION) != null)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<Person> Convert(string pathToImportedFile)
        {
            var lines = File.ReadAllText(pathToImportedFile);
            var contacts = Regex.Split(lines, END);

            foreach (var contact in contacts)
            {
                if(TryConvert(contact, out Person person))
                {
                    yield return person;
                }
            }
        }

        private bool TryConvert(string contact, out Person person)
        {
            person = null;

            var match = NameBirthdayRegex.Value.Match(contact);
            if (match.Success)
            {
                var name = match.Groups[NAME].Value;
                var birthday = match.Groups[BIRTHDAY].Value;
                var (date, isYearSet) = StringToDateTime(birthday);
                person = Person.Factory.CreatePerson(name, date, isYearSet);
                return true;
            }

            return false;
        }

        private (DateTime date, bool isYearSet) StringToDateTime(string birthday)
        {
            // either 19991231 or --1231
            int year, month, day;

            if (birthday.Length == 8) {
                year = int.Parse(birthday.Substring(0, 4));
                month = int.Parse(birthday.Substring(4, 2));
                day = int.Parse(birthday.Substring(6, 2));
                return (new DateTime(year, month, day), true);
            }
            else if (birthday.Length == 6)
            {
                month = int.Parse(birthday.Substring(2, 2));
                day = int.Parse(birthday.Substring(4, 2));
                return (new DateTime(2000, month, day), false);
            }
            else throw new ArgumentException(
                $"{birthday} is in invalid format (either 'yyyymmdd' or '--mmyy')", 
                nameof(birthday)
                );
        }
    }
}

// "BEGIN:VCARD\r\nVERSION:3.0\r\nFN:Adam Smith\r\nN:Smith;Adam;;;\r\nEMAIL;TYPE=INTERNET;TYPE=WORK:A.Smith@yahoo.com\r\nEMAIL;TYPE=INTERNET;TYPE=HOME:1234567\r\nTEL;TYPE=CELL:+22334453724\r\nTEL;TYPE=HOME:322-233-773\r\nADR;TYPE=HOME:;Washingtow 22/22;;;;;\r\nBDAY:19890918\r\n"
// "\r\nBEGIN:VCARD\r\nVERSION:3.0\r\nFN:John Doe\r\nN:Doe;John;;;\r\nTEL;TYPE=CELL:+99 234 158 234\r\nBDAY:--0404\r\n"
// "\r\nBEGIN:VCARD\r\nVERSION:3.0\r\nFN:Amely Qwerty\r\nN:Qwerty;Amely;;;\r\nBDAY:19380411\r\n"
// "\r\nBEGIN:VCARD\r\nVERSION:3.0\r\nFN:Adam Adam\r\nN:Adam;Adam;;;\r\nTEL;TYPE=CELL:666-777-888\r\n"
