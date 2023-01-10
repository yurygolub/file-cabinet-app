using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.CommandHandlers
{
    public class CreateCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public CreateCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "create")
            {
                Program.InputRecord(out RecordParameterObject record);
                int id = this.fileCabinetService.CreateRecord(record);
                Console.WriteLine($"Record #{id} is created.");
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
