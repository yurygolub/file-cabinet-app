using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHandler : ServiceCommandHandlerBase
    {
        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "remove")
            {
                if (!int.TryParse(request.Parameters, out int id))
                {
                    Console.WriteLine($"Couldn't parse '{request.Parameters}'.");
                    return null;
                }

                if (this.service.Remove(id))
                {
                    Console.WriteLine($"Record #{id} is removed.");
                }
                else
                {
                    Console.WriteLine($"Record #{id} doesn't exist.");
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
