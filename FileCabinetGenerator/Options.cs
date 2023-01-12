using CommandLine;

namespace FileCabinetGenerator
{
    public class Options
    {
        [Option('t', "output-type", Required = true, HelpText = "Output file type")]
        public string OutputType { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file name")]
        public string OutputFileName { get; set; }

        [Option('a', "records-amount", Required = true, HelpText = "Records amount")]
        public int RecordsAmount { get; set; }

        [Option('i', "start-id", Required = true, HelpText = "Start id")]
        public int StartId { get; set; }
    }
}
