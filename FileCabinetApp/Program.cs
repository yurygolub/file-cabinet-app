using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Converters;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Snapshot;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Yury Golub";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string PathToDB = "cabinet-records.db";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a record", "The 'create' command creates a record." },
            new string[] { "list", "returns list of records added to service", "The 'list' command returns list of records added to service." },
            new string[] { "edit", "edits a record", "The 'edit' command edits a record." },
            new string[] { "find", "finds a record", "The 'find' command finds a record." },
            new string[] { "export", "exports service data in specified format", "The 'export' command exports service data in specified format." },
            new string[]
            {
                "import", "imports records from specified format, the imported records will added to the existing records, " +
                "if the record with the specified id already exists in the storage, the existing record will be overwritten",
                "The 'import' command imports records from specified format.",
            },
            new string[] { "remove", "removes records", "The 'remove' command removes records." },
            new string[] { "purge", "defragmentates file", "The 'purge' command defragmentates file." },
        };

        private static readonly Tuple<string, string, string[], Action<string>>[] CommandLineParameters = new[]
        {
            new Tuple<string, string, string[], Action<string>>("--storage", "-s", new string[] { "memory", "file" }, SetStorage),
            new Tuple<string, string, string[], Action<string>>("--validation-rules", "-v", new string[] { "default", "custom" }, SetValidationRule),
        };

        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        private static IValidator inputValidator = new DefaultInputValidator();

        /// <summary>
        /// The entry point of application.
        /// </summary>
        /// <param name="args">Application command line parameter.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            CommandHandler(args);
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void CommandHandler(string[] args)
        {
            Dictionary<string, string> commands = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    string[] splitted = args[i].Split("=", 2);
                    if (splitted.Length == 2)
                    {
                        HandleArgs(commands, splitted[0], splitted[1]);
                    }
                }
                else if (args[i].StartsWith("-"))
                {
                    string value = args.Length > i + 1 ? args[i + 1] : string.Empty;
                    HandleArgs(commands, args[i], value);
                }
            }

            for (int i = 0; i < CommandLineParameters.Length; i++)
            {
                string key1 = CommandLineParameters[i].Item1, key2 = CommandLineParameters[i].Item2;
                if (commands.ContainsKey(key1))
                {
                    CommandLineParameters[i].Item4(commands[key1]);
                }
                else if (commands.ContainsKey(key2))
                {
                    CommandLineParameters[i].Item4(commands[key2]);
                }
                else
                {
                    CommandLineParameters[i].Item4(string.Empty);
                }
            }

            static void HandleArgs(Dictionary<string, string> commands, string key, string value)
            {
                string lowerKey = key.ToLower(), lowerValue = value.ToLower();
                if (ValidateArgs(lowerKey, lowerValue))
                {
                    if (commands.ContainsKey(lowerKey))
                    {
                        commands[lowerKey] = lowerValue;
                    }
                    else
                    {
                        commands.Add(lowerKey, lowerValue);
                    }
                }
            }

            static bool ValidateArgs(string key, string value)
            {
                for (int i = 0; i < CommandLineParameters.Length; i++)
                {
                    if (CommandLineParameters[i].Item1 == key || CommandLineParameters[i].Item2 == key)
                    {
                        int index = Array.FindIndex(CommandLineParameters[i].Item3, (str) => str == value);
                        return index != -1;
                    }
                }

                return false;
            }
        }

        private static void SetStorage(string arg)
        {
            switch (arg)
            {
                case "file":
                    fileCabinetService = new FileCabinetFilesystemService(OpenFile());
                    Console.WriteLine("Using file.");
                    break;

                case "memory":
                default:
                    fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                    Console.WriteLine("Using memory.");
                    break;
            }
        }

        private static void SetValidationRule(string arg)
        {
            switch (arg)
            {
                case "custom":
                    SetValidationRule(new CustomValidator());
                    inputValidator = new CustomInputValidator();
                    Console.WriteLine("Using custom validation rules.");
                    break;

                case "default":
                default:
                    SetValidationRule(new DefaultValidator());
                    inputValidator = new DefaultInputValidator();
                    Console.WriteLine("Using default validation rules.");
                    break;
            }

            static void SetValidationRule(IRecordValidator recordValidator)
            {
                if (fileCabinetService is FileCabinetMemoryService)
                {
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                }
            }
        }

        private static FileStream OpenFile()
        {
            if (!File.Exists(PathToDB))
            {
                throw new FileNotFoundException($"File '{PathToDB}' not found.");
            }

            return new FileStream(PathToDB, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;

            if (fileCabinetService is FileCabinetFilesystemService filesystemService)
            {
                filesystemService.Dispose();
            }
        }

        private static void Stat(string parameters)
        {
            if (fileCabinetService is FileCabinetFilesystemService filesystemService)
            {
                int count = filesystemService.GetStat();
                int removed = filesystemService.CountOfRemoved();
                Console.WriteLine($"{count} record(s). {removed} removed.");
            }
            else
            {
                int recordsCount = Program.fileCabinetService.GetStat();
                Console.WriteLine($"{recordsCount} record(s).");
            }
        }

        private static void Create(string parameters)
        {
            InputRecord(out RecordParameterObject record);
            int id = fileCabinetService.CreateRecord(record);
            Console.WriteLine($"Record #{id} is created.");
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] records = (FileCabinetRecord[])fileCabinetService.GetRecords();
            PrintRecords(records);
        }

        private static void PrintRecords(FileCabinetRecord[] records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth.ToString("yyyy'-'MMM'-'dd", System.Globalization.CultureInfo.InvariantCulture)}, " +
                    $"{record.Weight}, {record.Account}, {record.Letter}");
            }
        }

        private static void Edit(string parameters)
        {
            if (!int.TryParse(parameters, out int id))
            {
                Console.WriteLine($"The '{parameters}' is incorrect parameters");
                return;
            }

            try
            {
                fileCabinetService.IsRecordExist(id);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            InputRecord(out RecordParameterObject record);
            fileCabinetService.EditRecord(id, record);
            Console.WriteLine($"Record #{id} is updated.");
        }

        private static void InputRecord(out RecordParameterObject record)
        {
            IConverter converter = new Converter();

            Console.Write("First name: ");
            string firstName = ReadInput(converter.StringConvert, inputValidator.FirstNameValidate);

            Console.Write("Last name: ");
            string lastName = ReadInput(converter.StringConvert, inputValidator.LastNameValidate);

            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(converter.DateConvert, inputValidator.DateOfBirtheValidate);

            Console.Write("Weight: ");
            short weight = ReadInput(converter.ShortConvert, inputValidator.WeightValidate);

            Console.Write("Account: ");
            decimal account = ReadInput(converter.DecimalConvert, inputValidator.AccountValidate);

            Console.Write("Letter: ");
            char letter = ReadInput(converter.CharConvert, inputValidator.LetterValidate);

            record = new RecordParameterObject(firstName, lastName, dateOfBirth, weight, account, letter);
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                string input = Console.ReadLine();
                Tuple<bool, string, T> conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                Tuple<bool, string> validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static void Find(string parameters)
        {
            const int IndexPropertyName = 0;
            const int IndexOfTextToSearch = 1;

            if (string.IsNullOrEmpty(parameters))
            {
                Console.WriteLine($"You should write the parameters.");
                return;
            }

            string[] arrayOfParameters = parameters.Split(" ", 2);
            if (arrayOfParameters.Length < 2)
            {
                Console.WriteLine($"You should write the text to search for.");
                return;
            }

            FileCabinetRecord[] result = Array.Empty<FileCabinetRecord>();
            if (string.Equals("firstname", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
            {
                result = (FileCabinetRecord[])fileCabinetService.FindByFirstName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
            }
            else if (string.Equals("lastname", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
            {
                result = (FileCabinetRecord[])fileCabinetService.FindByLastName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
            }
            else if (string.Equals("dateofbirth", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
            {
                if (DateTime.TryParse(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty), out DateTime dateOfBirth))
                {
                    result = (FileCabinetRecord[])fileCabinetService.FindByDateOfBirth(dateOfBirth);
                }
                else
                {
                    Console.WriteLine($"The following date '{arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty)}' has incorrect format.");
                }
            }
            else
            {
                Console.WriteLine($"The '{arrayOfParameters[IndexPropertyName]}' property is not exist.");
            }

            PrintRecords(result);
        }

        private static void Export(string input)
        {
            FileCabinetServiceSnapshot fileCabinetServiceSnapshot = fileCabinetService.MakeSnapshot();

            Tuple<string, Action<StreamWriter>>[] fileFormats = new Tuple<string, Action<StreamWriter>>[]
            {
                new Tuple<string, Action<StreamWriter>>("csv", fileCabinetServiceSnapshot.SaveToCsv),
                new Tuple<string, Action<StreamWriter>>("xml", fileCabinetServiceSnapshot.SaveToXml),
            };

            string[] parameters = input.Split(' ', 2);
            const int fileFormatIndex = 0;
            string fileFormat = parameters[fileFormatIndex];

            if (string.IsNullOrEmpty(fileFormat))
            {
                Console.WriteLine("You should write the parameters.");
                return;
            }

            if (parameters.Length < 2)
            {
                Console.WriteLine("You should write the file name.");
                return;
            }

            int index = Array.FindIndex(fileFormats, 0, fileFormats.Length, i => i.Item1.Equals(fileFormat, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                const int fileNameIndex = 1;
                string fileName = parameters[fileNameIndex];
                SaveToFile(fileName, fileFormats[index].Item2);
            }
            else
            {
                Console.WriteLine($"There is no '{fileFormat}' file format.");
            }
        }

        private static void SaveToFile(string fileName, Action<StreamWriter> format)
        {
            if (File.Exists(fileName))
            {
                Console.Write($"File is exist - rewrite {fileName}? [Y/n] ");
                string input = Console.ReadLine();
                if (input == "Y")
                {
                    Write(fileName, format);
                }
                else if (input == "n")
                {
                    return;
                }
            }
            else
            {
                Write(fileName, format);
            }

            static void Write(string fileName, Action<StreamWriter> format)
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.Default);
                    format(streamWriter);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                Console.WriteLine($"All records are exported to file {fileName}.");
            }
        }

        private static void Import(string input)
        {
            FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();

            Tuple<string, Action<StreamReader>>[] fileFormats = new Tuple<string, Action<StreamReader>>[]
            {
                new Tuple<string, Action<StreamReader>>("csv", snapshot.LoadFromCsv),
                new Tuple<string, Action<StreamReader>>("xml", snapshot.LoadFromXml),
            };

            string[] parameters = input.Split(' ', 2);
            const int fileFormatIndex = 0;
            string fileFormat = parameters[fileFormatIndex];

            if (string.IsNullOrEmpty(fileFormat))
            {
                Console.WriteLine("You should write the parameters.");
                return;
            }

            if (parameters.Length < 2)
            {
                Console.WriteLine("You should write the file name.");
                return;
            }

            int index = Array.FindIndex(fileFormats, 0, fileFormats.Length, i => i.Item1.Equals(fileFormat, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                const int fileNameIndex = 1;
                string fileName = parameters[fileNameIndex];

                if (!LoadFromFile(fileName, fileFormats[index].Item2))
                {
                    return;
                }

                int count = fileCabinetService.Restore(snapshot);

                Console.WriteLine($"{count} records were imported from {fileName}.");
            }
            else
            {
                Console.WriteLine($"There is no '{fileFormat}' file format.");
            }
        }

        private static bool LoadFromFile(string fileName, Action<StreamReader> format)
        {
            try
            {
                using FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using StreamReader streamReader = new StreamReader(fileStream);

                format(streamReader);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private static void Remove(string arg)
        {
            if (!int.TryParse(arg, out int id))
            {
                Console.WriteLine($"Couldn't parse '{arg}'.");
                return;
            }

            if (fileCabinetService.Remove(id))
            {
                Console.WriteLine($"Record #{id} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{id} doesn't exist.");
            }
        }

        private static void Purge(string args)
        {
            if (fileCabinetService is FileCabinetFilesystemService filesystemService)
            {
                int recordsCount = fileCabinetService.GetStat();
                int recordsPurged = filesystemService.Purge();
                Console.WriteLine($"Data file processing is completed: {recordsPurged} of {recordsCount} records were purged.");
            }
        }
    }
}
