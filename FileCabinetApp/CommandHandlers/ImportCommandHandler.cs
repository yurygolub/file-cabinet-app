using System;
using System.IO;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Snapshot;

namespace FileCabinetApp.CommandHandlers
{
    public class ImportCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public ImportCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "import")
            {
                FileCabinetServiceSnapshot snapshot = this.fileCabinetService.MakeSnapshot();

                Tuple<string, Action<StreamReader>>[] fileFormats = new Tuple<string, Action<StreamReader>>[]
                {
                new Tuple<string, Action<StreamReader>>("csv", snapshot.LoadFromCsv),
                new Tuple<string, Action<StreamReader>>("xml", snapshot.LoadFromXml),
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

                    if (!LoadFromFile(fileName, fileFormats[index].Item2))
                    {
                        return null;
                    }

                    int count = this.fileCabinetService.Restore(snapshot);

                    Console.WriteLine($"{count} records were imported from {fileName}.");
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

        private static bool LoadFromFile(string fileName, Action<StreamReader> format)
        {
            try
            {
                using FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                using StreamReader streamReader = new StreamReader(fileStream);

                format(streamReader);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
    }
}
