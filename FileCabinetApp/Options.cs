using CommandLine;

namespace FileCabinetApp
{
    public class Options
    {
        [Option('s', "storage", Required = false, HelpText = "Set storage")]
        public string Storage { get; set; }

        [Option('v', "validation-rules", Required = false, HelpText = "Set validation rules")]
        public string ValidationRules { get; set; }
    }
}
