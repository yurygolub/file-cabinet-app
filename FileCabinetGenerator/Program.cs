using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Bogus;
using CommandLine;
using FileCabinetGenerator.Serialization;

namespace FileCabinetGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Options options = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(HandleParsed)
                .WithNotParsed(err => Environment.Exit(0)).Value;

            var records = GenerateRecords(options.RecordsAmount, options.StartId);

            Export(options.OutputFileName, options.OutputType, records);
        }

        private static void HandleParsed(Options options)
        {
            if (options.RecordsAmount < 0)
            {
                Console.WriteLine("Amount cannot be less than zero.");
                Environment.Exit(0);
            }

            if (options.StartId < 1)
            {
                Console.WriteLine("Start id cannot be less than one.");
                Environment.Exit(0);
            }
        }

        private static void Export(string fileName, string fileFormat, IEnumerable<FileCabinetRecord> records)
        {
            if (string.Equals(fileFormat, "csv", StringComparison.OrdinalIgnoreCase))
            {
                SaveToFile($"{fileName}.{fileFormat}", records, SaveToCsv);
            }
            else if (string.Equals(fileFormat, "xml", StringComparison.OrdinalIgnoreCase))
            {
                SaveToFile($"{fileName}.{fileFormat}", records, SaveToXml);
            }
        }

        private static void SaveToFile(string fileName, IEnumerable<FileCabinetRecord> records, Action<IEnumerable<FileCabinetRecord>, StreamWriter> saveToFile)
        {
            if (File.Exists(fileName))
            {
                Console.Write($"File is exist - rewrite {fileName}? [Y/n] ");
                string input = Console.ReadLine();
                if (input == "Y")
                {
                    Save(fileName, records, saveToFile);
                }
                else if (input == "n")
                {
                    return;
                }
            }
            else
            {
                Save(fileName, records, saveToFile);
            }

            static void Save(string fileName, IEnumerable<FileCabinetRecord> records, Action<IEnumerable<FileCabinetRecord>, StreamWriter> saveToFile)
            {
                using FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                using StreamWriter streamWriter = new StreamWriter(fileStream);
                saveToFile(records, streamWriter);

                Console.WriteLine($"All records are exported to file {fileName}.");
            }
        }

        private static void SaveToCsv(IEnumerable<FileCabinetRecord> records, StreamWriter streamWriter)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            streamWriter.WriteLine("Id,First Name,Last Name,Date of Birth,Weight,Account,Letter");
            foreach (var record in records)
            {
                WriteRecord(record, streamWriter);
            }

            static void WriteRecord(FileCabinetRecord fileCabinetRecord, StreamWriter streamWriter)
            {
                streamWriter.WriteLine($"{fileCabinetRecord.Id},{fileCabinetRecord.FirstName},{fileCabinetRecord.LastName}," +
                        $"{fileCabinetRecord.DateOfBirth.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)}," +
                        $"{fileCabinetRecord.Weight},{fileCabinetRecord.Account},{fileCabinetRecord.Letter}");
            }
        }

        private static void SaveToXml(IEnumerable<FileCabinetRecord> records, StreamWriter streamWriter)
        {
            _ = records ?? throw new ArgumentNullException(nameof(records));
            _ = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));

            FileCabinetRecordSerializable[] fileCabinetRecords = records
                .Select(r =>
                    new FileCabinetRecordSerializable
                    {
                        Id = r.Id,
                        FullName = new FullName
                        {
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                        },
                        DateOfBirth = r.DateOfBirth.ToString("dd/MM/yyyy"),
                        Weight = r.Weight,
                        Account = r.Account,
                        Letter = r.Letter,
                    })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileCabinetRecordSerializable[]), new XmlRootAttribute("records"));

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
            };

            using XmlWriter xmlWriter = XmlWriter.Create(streamWriter, xmlWriterSettings);

            xmlSerializer.Serialize(xmlWriter, fileCabinetRecords);
        }

        private static IEnumerable<FileCabinetRecord> GenerateRecords(int amount, int startId)
        {
            return new Faker<FileCabinetRecord>("en")
                .RuleFor(r => r.Id, f => startId++)
                .RuleFor(r => r.FirstName, f => f.Name.FirstName())
                .RuleFor(r => r.LastName, f => f.Name.LastName())
                .RuleFor(r => r.DateOfBirth, f => f.Person.DateOfBirth)
                .RuleFor(r => r.Weight, f => f.Random.Short(40, 120))
                .RuleFor(r => r.Account, f => f.Random.Int(0, 100_000) / 100m)
                .RuleFor(r => r.Letter, f => (char)f.Random.Int(97, 122))
                .Generate(amount);
        }
    }
}
