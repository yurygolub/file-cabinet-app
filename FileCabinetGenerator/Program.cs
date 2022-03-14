using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetGenerator.Configuration;
using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator
{
    public class Program
    {
        private static readonly Command<CommandType>[] CommandLineParameters = new[]
        {
            new Command<CommandType>(new[] { "--output-type", "-t" }, CommandType.OutputType),
            new Command<CommandType>(new[] { "--output", "-o" }, CommandType.Output),
            new Command<CommandType>(new[] { "--records-amount", "-a" }, CommandType.RecordsAmount),
            new Command<CommandType>(new[] { "--start-id", "-i" }, CommandType.StartId),
        };

        private enum CommandType
        {
            OutputType,
            Output,
            RecordsAmount,
            StartId,
        }

        public static void Main(string[] args)
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddCustomCommandLine(args)
                .Build();

            var parameters = HandleParameters(configurationRoot);

            var records = GenerateRecords(parameters.amount, parameters.startId);

            Export(parameters.fileName, parameters.fileFormat, records);
        }

        private static void Export(string fileName, string fileFormat, IEnumerable<FileCabinetRecord> records)
        {
            if (string.Equals(fileFormat, "csv", StringComparison.OrdinalIgnoreCase))
            {
                SaveToFile($"{fileName}.{fileFormat}", records, SaveToCsv);
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

        private static IEnumerable<FileCabinetRecord> GenerateRecords(int amount, int startId)
        {
            const string firstNamesPath = "first names.txt";
            const string lastNamesPath = "last names.txt";

            string[] firstNames = ReadFile(firstNamesPath);
            string[] lastNames = ReadFile(lastNamesPath);

            return GenerateRecord(amount, startId, firstNames, lastNames);

            static IEnumerable<FileCabinetRecord> GenerateRecord(int amount, int startId,  string[] firstNames, string[] lastNames)
            {
                Random random = new Random();
                for (int currentId = startId; currentId < amount; currentId++)
                {
                    yield return new FileCabinetRecord
                    {
                        Id = startId,
                        FirstName = GenerateName(firstNames),
                        LastName = GenerateName(lastNames),
                        DateOfBirth = GenerateDate(),
                        Weight = (short)random.Next(40, 120),
                        Account = (decimal)Math.Round(random.NextDouble() * random.Next(1_000), 2),
                        Letter = (char)new Random().Next(97, 123),
                    };
                }

                static string GenerateName(string[] names)
                {
                    int index = new Random().Next(names.Length);
                    return names[index];
                }

                static DateTime GenerateDate()
                {
                    Random random = new Random();
                    int year = random.Next(1950, DateTime.Now.Year);
                    int month = random.Next(1, 12);
                    int day = random.Next(1, 29);
                    return new DateTime(year, month, day);
                }
            }
        }

        private static string[] ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{nameof(path)} is not found.", nameof(path));
            }

            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new StreamReader(fileStream);

            List<string> data = new List<string>();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                data.Add(line);
            }

            return data.ToArray();
        }

        private static (string fileFormat, string fileName, int amount, int startId) HandleParameters(IConfigurationRoot configurationRoot)
        {
            (string fileFormat, string fileName, int amount, int startId) result = default;

            foreach (var command in CommandLineParameters)
            {
                string value = GetValue(configurationRoot, command);
                switch (command.CommandType)
                {
                    case CommandType.OutputType:
                        if (value is null)
                        {
                            throw new ArgumentException("You must specify output type.");
                        }

                        result.fileFormat = value;
                        break;

                    case CommandType.Output:
                        if (value is null)
                        {
                            throw new ArgumentException("You must specify output file name.");
                        }

                        result.fileName = value;
                        break;

                    case CommandType.RecordsAmount:
                        if (value is null)
                        {
                            throw new ArgumentException("You must specify amount.");
                        }

                        int amount = int.Parse(value);
                        if (amount < 0)
                        {
                            throw new ArgumentException("amount cannot be less than zero.");
                        }

                        result.amount = amount;
                        break;

                    case CommandType.StartId:
                        if (value is null)
                        {
                            throw new ArgumentException("You must specify start id.");
                        }

                        int startId = int.Parse(value);
                        if (startId < 1)
                        {
                            throw new ArgumentException("startId cannot be less than one.");
                        }

                        result.startId = startId;
                        break;
                }
            }

            return result;

            static string GetValue(IConfigurationRoot configurationRoot, Command<CommandType> command)
            {
                string value = null;
                foreach (string name in command.Names)
                {
                    value = configurationRoot[name];
                    if (value != null)
                    {
                        break;
                    }
                }

                return value;
            }
        }

        private class Command<T>
            where T : Enum
        {
            private readonly List<string> names;

            public Command(IEnumerable<string> names, T commandType)
            {
                if (names is null)
                {
                    throw new ArgumentNullException(nameof(names));
                }

                this.names = new List<string>(names);
                this.CommandType = commandType;
            }

            public IEnumerable<string> Names => this.names;

            public T CommandType { get; }
        }
    }
}
