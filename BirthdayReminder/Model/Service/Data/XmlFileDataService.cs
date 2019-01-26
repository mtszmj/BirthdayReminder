using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BirthdayReminder.Model.Service
{
    public class XmlFileDataService : IDataService
    {
        public XmlFileDataService(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public string Path { get; set; }

        public IEnumerable<Person> GetPeople()
        {
            if (File.Exists(Path))
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<Person>));
                using (var reader = new StreamReader(Path))
                {
                    var people = serializer.Deserialize(reader) as ObservableCollection<Person>;
                    return people;
                }
            }
            else return Enumerable.Empty<Person>();
        }

        public void SavePeople(ObservableCollection<Person> people)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            var serializer = new XmlSerializer(typeof(ObservableCollection<Person>));
            using (var writer = new StreamWriter(Path))
            {
                serializer.Serialize(writer, people);
            }
        }
    }
}
