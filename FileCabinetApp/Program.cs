using System;
using System.Collections.Generic;
using System.Globalization;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Converters;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;
using FileCabinetApp.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FileCabinetApp
{
    /// <summary>
    /// Main program class.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Yury Golub";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static readonly Startup Startup = new ();

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator recordValidator;
        private static IValidator inputValidator;
        private static bool isRunning = true;

        /// <summary>
        /// The entry point of application.
        /// </summary>
        /// <param name="args">Application command line parameter.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");

            Parser.Default.ParseArguments<Options>(args)
              .WithParsed(CommandHandler)
              .WithNotParsed(err => Environment.Exit(0));

            Console.WriteLine(Program.HintMessage);

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

        private static void DefaultRecordPrint(IEnumerable<FileCabinetRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, " +
                    $"{record.DateOfBirth.ToString("yyyy'-'MMM'-'dd", CultureInfo.InvariantCulture)}, " +
                    $"{record.Weight}, {record.Account}, {record.Letter}");
            }
        }

        private static ICommandHandler CreateCommandHandlers()
        {
            var createHandler = new CreateCommandHandler(fileCabinetService);

            createHandler
                .SetNext(new EditCommandHandler(fileCabinetService))
                .SetNext(new ExitCommandHandler(fileCabinetService, () => isRunning = false))
                .SetNext(new ExportCommandHandler(fileCabinetService))
                .SetNext(new FindCommandHandler(fileCabinetService, DefaultRecordPrint))
                .SetNext(new HelpCommandHandler())
                .SetNext(new ImportCommandHandler(fileCabinetService))
                .SetNext(new ListCommandHandler(fileCabinetService, DefaultRecordPrint))
                .SetNext(new PurgeCommandHandler(fileCabinetService))
                .SetNext(new RemoveCommandHandler(fileCabinetService))
                .SetNext(new StatCommandHandler(fileCabinetService))
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

        private static void CommandHandler(Options opts)
        {
            switch (opts.ValidationRules)
            {
                case "custom":
                    recordValidator = new ValidatorBuilder().CreateCustom(Startup.Configuration);
                    inputValidator = new CustomInputValidator();
                    Console.WriteLine("Using custom validation rules.");
                    break;

                case "default":
                case "":
                    recordValidator = new ValidatorBuilder().CreateDefault(Startup.Configuration);
                    inputValidator = new DefaultInputValidator();
                    Console.WriteLine("Using default validation rules.");
                    break;

                default:
                    Console.WriteLine($"Wrong option: \'{opts.ValidationRules}\'");
                    Environment.Exit(0);
                    break;
            }

            switch (opts.Storage)
            {
                case "file":
                    fileCabinetService = Startup.ServiceProvider.GetService<FileCabinetFilesystemService>();
                    Console.WriteLine("Using file.");
                    break;

                case "memory":
                case "":
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                    Console.WriteLine("Using memory.");
                    break;

                default:
                    Console.WriteLine($"Wrong option: \'{opts.Storage}\'");
                    Environment.Exit(0);
                    break;
            }

            if (opts.UseStopwatch)
            {
                fileCabinetService = new ServiceMeter(fileCabinetService);
            }
        }
    }
}
