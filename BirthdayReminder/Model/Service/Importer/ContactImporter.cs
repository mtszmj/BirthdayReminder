using BirthdayReminder.Model.Service.Importer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace BirthdayReminder.Model.Service
{
    public class ContactImporter
    {
        private static Dictionary<string, ExtensionType> Extensions { get; } = new Dictionary<string, ExtensionType>
        {
            [".csv"] = ExtensionType.CSV,
            [".vcf"] = ExtensionType.VCF,
        };

        private IImportStrategy ImportStrategy;
        private string PathToImportedFile;

        private ContactImporter(IImportStrategy importStrategy, string pathToImportedFile)
        {
            ImportStrategy = importStrategy;
            PathToImportedFile = pathToImportedFile;
        }

        private enum ExtensionType
        {
            Unknown, CSV, VCF
        }

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
                var ext = Path.GetExtension(pathToImportedFile).ToLower();
                Extensions.TryGetValue(ext, out var extension);
                switch (extension)
                {
                    case ExtensionType.CSV:
                        return GetCsvStrategy(pathToImportedFile);
                    case ExtensionType.VCF:
                        return GetVcfStrategy(pathToImportedFile);
                    default:
                        return new NullStrategy();
                }
            }

            private static IImportStrategy GetCsvStrategy(string pathToImportedFile)
            {
                return GetStrategy(pathToImportedFile, typeof(ICsvImportStrategy));
            }

            private static IImportStrategy GetVcfStrategy(string pathToImportedFile)
            {
                return GetStrategy(pathToImportedFile, typeof(IVcfImportStrategy));
            }

            private static IImportStrategy GetStrategy(string pathToImportedFile, Type importInterface)
            {
                var importStrategies = Assembly.GetExecutingAssembly()
                    .GetTypes().Where(type => importInterface.IsAssignableFrom(type));

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
