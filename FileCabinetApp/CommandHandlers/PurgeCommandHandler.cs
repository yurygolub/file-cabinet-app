using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "purge")
            {
                if (this.service is FileCabinetFilesystemService filesystemService)
                {
                    int recordsCount = this.service.GetStat();
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
