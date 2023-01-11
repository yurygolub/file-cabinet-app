using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        public StatCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "stat")
            {
                if (this.service is FileCabinetFilesystemService filesystemService)
                {
                    int count = filesystemService.GetStat();
                    int removed = filesystemService.CountOfRemoved();
                    Console.WriteLine($"{count} record(s). {removed} removed.");
                }
                else
                {
                    int recordsCount = this.service.GetStat();
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
