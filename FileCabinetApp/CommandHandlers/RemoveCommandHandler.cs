using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class RemoveCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public RemoveCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
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

                if (this.fileCabinetService.Remove(id))
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
