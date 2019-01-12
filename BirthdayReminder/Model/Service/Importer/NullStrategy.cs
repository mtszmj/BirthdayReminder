using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service.Importer
{
    internal class NullStrategy : IImportStrategy
    {
        public IEnumerable<Person> Convert(string pathToImportedFile)
        {
            return Enumerable.Empty<Person>();
        }

        public bool IsCorrectFormat(string pathToFile)
        {
            return false;
        }
    }
}
