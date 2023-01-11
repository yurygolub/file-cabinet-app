using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        public ListCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
            : base(fileCabinetService)
        {
            this.printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "list")
            {
                var records = this.service.GetRecords();
                this.printer.Print(records);
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
