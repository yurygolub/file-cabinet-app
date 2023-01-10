using System;
using System.IO;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.CommandHandlers
{
    public class ExportCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "export")
            {
                FileCabinetServiceSnapshot fileCabinetServiceSnapshot = Program.fileCabinetService.MakeSnapshot();

                Tuple<string, Action<StreamWriter>>[] fileFormats = new Tuple<string, Action<StreamWriter>>[]
                {
                new Tuple<string, Action<StreamWriter>>("csv", fileCabinetServiceSnapshot.SaveToCsv),
                new Tuple<string, Action<StreamWriter>>("xml", fileCabinetServiceSnapshot.SaveToXml),
                };

                string[] parameters = request.Parameters.Split(' ', 2);
                const int fileFormatIndex = 0;
                string fileFormat = parameters[fileFormatIndex];

                if (string.IsNullOrEmpty(fileFormat))
                {
                    Console.WriteLine("You should write the parameters.");
                    return null;
                }

                if (parameters.Length < 2)
                {
                    Console.WriteLine("You should write the file name.");
                    return null;
                }

                int index = Array.FindIndex(fileFormats, 0, fileFormats.Length, i => i.Item1.Equals(fileFormat, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int fileNameIndex = 1;
                    string fileName = parameters[fileNameIndex];
                    SaveToFile(fileName, fileFormats[index].Item2);
                }
                else
                {
                    Console.WriteLine($"There is no '{fileFormat}' file format.");
                }
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }

        private static void SaveToFile(string fileName, Action<StreamWriter> format)
        {
            if (File.Exists(fileName))
            {
                Console.Write($"File is exist - rewrite {fileName}? [Y/n] ");
                string input = Console.ReadLine();
                if (input == "Y")
                {
                    Write(fileName, format);
                }
                else if (input == "n")
                {
                    return;
                }
            }
            else
            {
                Write(fileName, format);
            }

            static void Write(string fileName, Action<StreamWriter> format)
            {
                try
                {
                    using StreamWriter streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.Default);
                    format(streamWriter);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }

                Console.WriteLine($"All records are exported to file {fileName}.");
            }
        }
    }
}
