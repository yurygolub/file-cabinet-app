using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "purge")
            {
                if (Program.fileCabinetService is FileCabinetFilesystemService filesystemService)
                {
                    int recordsCount = Program.fileCabinetService.GetStat();
                    int recordsPurged = filesystemService.Purge();
                    Console.WriteLine($"Data file processing is completed: {recordsPurged} of {recordsCount} records were purged.");
                }
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
