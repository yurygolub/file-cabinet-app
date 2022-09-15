using Microsoft.Extensions.Configuration;

namespace FileCabinetGenerator.Configuration
{
    public class CommandLineConfigurationSource : IConfigurationSource
    {
        public CommandLineConfigurationSource(string[] args)
        {
            this.Args = args;
        }

        public string[] Args { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CommandLineConfigurationProvider(this.Args);
        }
    }
}
