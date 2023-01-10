using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Converters;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        public static bool isRunning = true;
        public static IFileCabinetService fileCabinetService = new FileCabinetMemoryService(new DefaultValidator());

        public static IValidator inputValidator = new DefaultInputValidator();

        private const string DeveloperName = "Yury Golub";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const string PathToDB = "cabinet-records.db";

        private static readonly Tuple<string, string, string[], Action<string>>[] CommandLineParameters = new[]
        {
            new Tuple<string, string, string[], Action<string>>("--storage", "-s", new string[] { "memory", "file" }, SetStorage),
            new Tuple<string, string, string[], Action<string>>("--validation-rules", "-v", new string[] { "default", "custom" }, SetValidationRule),
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

            ICommandHandler handler = CreateCommandHandlers();

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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                handler.Handle(new AppCommandRequest() { Command = command, Parameters = parameters });
            }
            while (isRunning);
        }

        public static void InputRecord(out RecordParameterObject record)
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

        public static void PrintRecords(IReadOnlyCollection<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth.ToString("yyyy'-'MMM'-'dd", System.Globalization.CultureInfo.InvariantCulture)}, " +
                    $"{record.Weight}, {record.Account}, {record.Letter}");
            }
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var createHandler = new CreateCommandHandler();

            createHandler
                .SetNext(new EditCommandHandler())
                .SetNext(new ExitCommandHandler())
                .SetNext(new ExportCommandHandler())
                .SetNext(new FindCommandHandler())
                .SetNext(new HelpCommandHandler())
                .SetNext(new ImportCommandHandler())
                .SetNext(new ListCommandHandler())
                .SetNext(new PurgeCommandHandler())
                .SetNext(new RemoveCommandHandler())
                .SetNext(new StatCommandHandler())
                .SetNext(new DefaultHandler());

            return createHandler;
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
    }
}
