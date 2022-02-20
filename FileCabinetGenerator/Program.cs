using System;
using System.Collections.Generic;
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

                        result.amount = int.Parse(value);
                        break;

                    case CommandType.StartId:
                        if (value is null)
                        {
                            throw new ArgumentException("You must specify start id.");
                        }

                        result.startId = int.Parse(value);
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
