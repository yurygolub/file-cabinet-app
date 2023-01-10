using System;

namespace FileCabinetApp.CommandHandlers
{
    public class ListCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "list")
            {
                var records = Program.fileCabinetService.GetRecords();
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
