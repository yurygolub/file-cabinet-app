using System;
using FileCabinetApp.Interfaces;
using FileCabinetApp.Record;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        public EditCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "edit")
            {
                if (!int.TryParse(request.Parameters, out int id))
                {
                    Console.WriteLine($"The '{request.Parameters}' is incorrect parameters");
                    return null;
                }

                Program.InputRecord(out RecordParameterObject record);
                if (!this.service.EditRecord(id, record))
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return null;
                }

                Console.WriteLine($"Record #{id} is updated.");
            }
            else
            {
                return base.Handle(request);
            }

            return request;
        }
    }
}
