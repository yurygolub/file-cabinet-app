﻿using System;

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
        private static FileCabinetService fileCabinetService = new FileCabinetDefaultService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
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
        };

        /// <summary>
        /// The entry point of application.
        /// </summary>
        /// <param name="args">Application command line parameter.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            SetValidationRule(args);
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

        private static void SetValidationRule(string[] args)
        {
            string temp = string.Join(" ", args);
            if (string.IsNullOrEmpty(temp))
            {
                Console.WriteLine("Using default validation rules.");
                return;
            }

            string[] commands = temp.Split(new string[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
            if (commands.Length < 2)
            {
                Console.WriteLine("Using default validation rules.");
                return;
            }

            const int IndexOfTypeValidationRules = 0;
            const int IndexOfValidationRules = 1;

            if (string.Equals("--validation-rules", commands[IndexOfTypeValidationRules]) || string.Equals("-v", commands[IndexOfTypeValidationRules]))
            {
                if (string.Equals("default", commands[IndexOfValidationRules], StringComparison.InvariantCultureIgnoreCase))
                {
                    fileCabinetService = new FileCabinetDefaultService();
                    Console.WriteLine("Using default validation rules.");
                    return;
                }

                if (string.Equals("custom", commands[IndexOfValidationRules], StringComparison.InvariantCultureIgnoreCase))
                {
                    fileCabinetService = new FileCabinetCustomService();
                    Console.WriteLine("Using custom validation rules.");
                    return;
                }
            }

            Console.WriteLine("Using default validation rules.");
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
            FileCabinetRecord[] records = fileCabinetService.GetRecords();
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
            while (true)
            {
                Console.Write("First name: ");
                string firstName = Console.ReadLine();

                Console.Write("Last name: ");
                string lastName = Console.ReadLine();

                Console.Write("Date of birth: ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
                {
                    Console.WriteLine("Incorrect format.");
                    continue;
                }

                Console.Write("Weight: ");
                if (!short.TryParse(Console.ReadLine(), out short weight))
                {
                    Console.WriteLine("Incorrect format.");
                    continue;
                }

                Console.Write("Account: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal account))
                {
                    Console.WriteLine("Incorrect format.");
                    continue;
                }

                Console.Write("Letter: ");
                if (!char.TryParse(Console.ReadLine(), out char letter))
                {
                    Console.WriteLine("Incorrect format.");
                    continue;
                }

                record = new Record(firstName, lastName, dateOfBirth, weight, account, letter);

                try
                {
                    fileCabinetService.CreateValidator().ValidateParameters(record);
                    break;
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
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
                result = fileCabinetService.FindByFirstName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
            }
            else if (string.Equals("lastname", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
            {
                result = fileCabinetService.FindByLastName(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty));
            }
            else if (string.Equals("dateofbirth", arrayOfParameters[IndexPropertyName], StringComparison.InvariantCultureIgnoreCase))
            {
                if (DateTime.TryParse(arrayOfParameters[IndexOfTextToSearch].Replace("\"", string.Empty), out DateTime dateOfBirth))
                {
                    result = fileCabinetService.FindByDateOfBirth(dateOfBirth);
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
    }
}