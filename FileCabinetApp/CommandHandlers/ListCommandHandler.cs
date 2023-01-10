using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public ListCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "list")
            {
                var records = this.fileCabinetService.GetRecords();
                Program.PrintRecords(records);
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
