using System;
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
                int count = this.service.GetStat();
                int removed = this.service.CountOfRemoved();
                Console.WriteLine($"{count} record(s). {removed} removed.");
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
