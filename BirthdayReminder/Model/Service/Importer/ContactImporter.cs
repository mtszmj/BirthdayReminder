using BirthdayReminder.Model.Service.Importer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BirthdayReminder.Model.Service
{
    public class ContactImporter
    {
        protected ContactImporter(IImportStrategy importStrategy, string pathToImportedFile)
        {
            ImportStrategy = importStrategy;
            PathToImportedFile = pathToImportedFile;
        }

        private IImportStrategy ImportStrategy;
        private string PathToImportedFile;

        public IEnumerable<Person> Import()
        {
            if (PathToImportedFile == null)
                throw new ArgumentNullException(nameof(PathToImportedFile));
            if (!File.Exists(PathToImportedFile))
                throw new ArgumentException("File does not exist", nameof(PathToImportedFile));

            return ImportStrategy.Convert(PathToImportedFile);
        }

        public static class Factory
        {
            public static ContactImporter CreateFor(string pathToImportedFile)
            {
                if (pathToImportedFile == null)
                    throw new ArgumentNullException(nameof(pathToImportedFile));
                if (!File.Exists(pathToImportedFile))
                    throw new ArgumentException("File does not exist", nameof(PathToImportedFile));

                return new ContactImporter(FindFormat(pathToImportedFile), pathToImportedFile);
            }

            private static IImportStrategy FindFormat(string pathToImportedFile)
            {
                var extension = Path.GetExtension(pathToImportedFile).ToLower();
                switch(extension)
                {
                    case ".csv":
                        return FindCsvFormat(pathToImportedFile);
                    case ".vcf":
                        return new Vcf3Strategy();
                    default:
                        return new NullStrategy();
                }
            }

            private static IImportStrategy FindCsvFormat(string pathToImportedFile)
            {
                string firstLine;
                using (var reader = new StreamReader(pathToImportedFile))
                {
                    firstLine = reader.ReadLine();
                }
                
                var importStrategies = Assembly.GetExecutingAssembly()
                    .GetTypes().Where(type => typeof(IImportStrategy).IsAssignableFrom(type));

                foreach (var strategy in importStrategies)
                {
                    if (typeof(IImportStrategy).IsAssignableFrom(strategy) && strategy.IsClass)
                    {
                        var importer = (IImportStrategy)Activator.CreateInstance(strategy);
                        if (importer.IsCorrectFormat(pathToImportedFile))
                            return importer;
                    }
                }

                return new NullStrategy();
            }
        }
    }
}
