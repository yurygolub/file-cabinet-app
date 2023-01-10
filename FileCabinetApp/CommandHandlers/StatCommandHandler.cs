using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public StatCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "stat")
            {
                if (this.fileCabinetService is FileCabinetFilesystemService filesystemService)
                {
                    int count = filesystemService.GetStat();
                    int removed = filesystemService.CountOfRemoved();
                    Console.WriteLine($"{count} record(s). {removed} removed.");
                }
                else
                {
                    int recordsCount = this.fileCabinetService.GetStat();
                    Console.WriteLine($"{recordsCount} record(s).");
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
