﻿using System;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Yury Golub";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
        private static IValidator validator = new DefValidator();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "displays statistics on records", "The 'stat' command displays statistics on records." },
            new string[] { "create", "creates a record", "The 'create' command creates a record." },
            new string[] { "list", "returns list of records added to service", "The 'list' command returns list of records added to service." },
            new string[] { "edit", "edits a record", "The 'edit' command edits a record." },
            new string[] { "find", "finds a record", "The 'find' command finds a record." },
            new string[] { "export", "exports service data in specified format", "The 'export' command exports service data in specified format." },
        };

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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
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
            Tuple<string[], Action<string>>[] commandHandler = new Tuple<string[], Action<string>>[]
            {
                new Tuple<string[], Action<string>>(new string[] { "--storage", "-s" }, SetStorage),
                new Tuple<string[], Action<string>>(new string[] { "--validation-rules", "-v" }, SetValidationRule),
            };

            string temp = string.Join(" ", args);
            string[] commands = temp.Split(new string[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
            (string, string)[] commandsWithArgs = new (string, string)[commandHandler.Length];
            for (int i = 0, j = 0; i < commands.Length && j < commandsWithArgs.Length; i += 2, j++)
            {
                commandsWithArgs[j].Item1 = commands[i];
                commandsWithArgs[j].Item2 = i + 1 < commands.Length ? commands[i + 1] : string.Empty;
            }

            for (int i = 0; i < commandHandler.Length; i++)
            {
                int index = Array.FindIndex(commandsWithArgs, tuple => Array.Exists(commandHandler[i].Item1, str => str.Equals(tuple.Item1, StringComparison.InvariantCultureIgnoreCase)));
                commandHandler[i].Item2(index >= 0 ? commandsWithArgs[index].Item2 : string.Empty);
            }
        }

        private static void SetValidationRule(string arg)
        {
            switch (arg)
            {
                case "custom":
                    SetValRule(new CustomValidator());
                    validator = new CusValidator();
                    Console.WriteLine("Using custom validation rules.");
                    break;

                case "default":
                default:
                    SetValRule(new DefaultValidator());
                    validator = new DefValidator();
                    Console.WriteLine("Using default validation rules.");
                    break;
            }

            void SetValRule(IRecordValidator recordValidator)
            {
                if (fileCabinetService is FileCabinetMemoryService)
                {
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                }
                else if (fileCabinetService is FileCabinetFilesystemService)
                {
                    ((FileCabinetFilesystemService)fileCabinetService).CloseFile();
                    fileCabinetService = new FileCabinetFilesystemService(recordValidator);
                    ((FileCabinetFilesystemService)fileCabinetService).OpenFile();
                }
            }
        }

        private static void SetStorage(string arg)
        {
            switch (arg)
            {
                case "file":
                    fileCabinetService = new FileCabinetFilesystemService(new DefaultValidator());
                    if (fileCabinetService is FileCabinetFilesystemService)
                    {
                        ((FileCabinetFilesystemService)fileCabinetService).OpenFile();
                    }

                    Console.WriteLine("Using file.");
                    break;

                case "memory":
                default:
                    fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());
                    Console.WriteLine("Using memory.");
                    break;
            }
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
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

            if (fileCabinetService is FileCabinetFilesystemService)
            {
                ((FileCabinetFilesystemService)fileCabinetService).CloseFile();
            }
        }

        private static void Stat(string parameters)
        {
            int recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            InputRecord(out Record record);
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
                fileCabinetService.IsExist(id);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            InputRecord(out Record record);
            fileCabinetService.EditRecord(id, record);
            Console.WriteLine($"Record #{id} is updated.");
        }

        private static void InputRecord(out Record record)
        {
            IConverter converter = new Converter();

            Console.Write("First name: ");
            string firstName = ReadInput(converter.StringConvert, validator.FirstNameValidate);

            Console.Write("Last name: ");
            string lastName = ReadInput(converter.StringConvert, validator.LastNameValidate);

            Console.Write("Date of birth: ");
            DateTime dateOfBirth = ReadInput(converter.DateConvert, validator.DateOfBirtheValidate);

            Console.Write("Weight: ");
            short weight = ReadInput(converter.ShortConvert, validator.WeightValidate);

            Console.Write("Account: ");
            decimal account = ReadInput(converter.DecimalConvert, validator.AccountValidate);

            Console.Write("Letter: ");
            char letter = ReadInput(converter.CharConvert, validator.LetterValidate);

            record = new Record(firstName, lastName, dateOfBirth, weight, account, letter);
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
                    Write();
                }
                else if (input == "n")
                {
                    return;
                }
            }
            else
            {
                Write();
            }

            void Write()
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.Default))
                    {
                        format(streamWriter);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                Console.WriteLine($"All records are exported to file {fileName}.");
            }
        }
    }
}