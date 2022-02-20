using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator.Configuration
{
    public class CommandLineConfigurationProvider : ConfigurationProvider
    {
        public CommandLineConfigurationProvider(string[] args)
        {
            this.Args = args;
        }

        public string[] Args { get; }

        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < this.Args.Length; i++)
            {
                if (this.Args[i].StartsWith("--"))
                {
                    string[] splitted = this.Args[i].Split("=", 2);
                    if (splitted.Length == 2)
                    {
                        HandleArgs(data, splitted[0], splitted[1]);
                    }
                }
                else if (this.Args[i].StartsWith("-"))
                {
                    if (this.Args.Length > i + 1)
                    {
                        HandleArgs(data, this.Args[i], this.Args[i + 1]);
                        i++;
                    }
                }
            }

            this.Data = data;

            static void HandleArgs(Dictionary<string, string> commands, string key, string value)
            {
                if (commands.ContainsKey(key))
                {
                    commands[key] = value;
                }
                else
                {
                    commands.Add(key, value);
                }
            }
        }
    }
}
