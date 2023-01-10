using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "purge")
            {
                if (this.fileCabinetService is FileCabinetFilesystemService filesystemService)
                {
                    int recordsCount = this.fileCabinetService.GetStat();
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
