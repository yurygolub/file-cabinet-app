using System;
using FileCabinetApp.Record;

namespace FileCabinetApp.CommandHandlers
{
    public class EditCommandHandler : CommandHandlerBase
    {
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

                try
                {
                    Program.fileCabinetService.IsRecordExist(id);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

                Program.InputRecord(out RecordParameterObject record);
                Program.fileCabinetService.EditRecord(id, record);
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
