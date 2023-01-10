using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    public class StatCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "stat")
            {
                if (Program.fileCabinetService is FileCabinetFilesystemService filesystemService)
                {
                    int count = filesystemService.GetStat();
                    int removed = filesystemService.CountOfRemoved();
                    Console.WriteLine($"{count} record(s). {removed} removed.");
                }
                else
                {
                    int recordsCount = Program.fileCabinetService.GetStat();
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
