using CommandLine;

namespace FileCabinetApp
{
    public class Options
    {
        [Option('s', "storage", Required = false, HelpText = "Set storage")]
        public string Storage { get; set; }

        [Option('v', "validation-rules", Required = false, HelpText = "Set validation rules")]
        public string ValidationRules { get; set; }

        [Option("use-stopwatch", Required = false, HelpText = "Enable measuring the execution time of service methods.")]
        public bool UseStopwatch { get; set; }

        [Option("use-logger", Required = false, HelpText = "Enable logging of service method calls.")]
        public bool UseLogger { get; set; }
    }
}
