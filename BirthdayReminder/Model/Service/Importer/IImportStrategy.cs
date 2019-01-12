using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayReminder.Model.Service.Importer
{
    public interface IImportStrategy
    {
        /// <summary>
        /// Check if file has a correct format for current strategy
        /// </summary>
        /// <param name="pathToImportedFile"></param>
        /// <returns>True if format is correct.</returns>
        bool IsCorrectFormat(string pathToImportedFile);

        /// <summary>
        /// Convert data from file to Collection of Person.
        /// </summary>
        /// <param name="pathToImportedFile"></param>
        /// <returns>A new Person</returns>
        IEnumerable<Person> Convert(string pathToImportedFile);
    }
}
