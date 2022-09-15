using System;
using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator.Configuration
{
    public static class CommandLineConfigurationExtensions
    {
        public static IConfigurationBuilder AddCustomCommandLine(this IConfigurationBuilder builder, string[] args)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (args is null)
            {
                throw new ArgumentException(nameof(args));
            }

            return builder.Add(new CommandLineConfigurationSource(args));
        }
    }
}
