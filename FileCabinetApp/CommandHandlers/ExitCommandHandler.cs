using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : ServiceCommandHandlerBase
    {
        private readonly Action exit;

        public ExitCommandHandler(IFileCabinetService fileCabinetService, Action exit)
            : base(fileCabinetService)
        {
            this.exit = exit;
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "exit")
            {
                Console.WriteLine("Exiting an application...");
                this.exit?.Invoke();

                if (this.service is FileCabinetFilesystemService filesystemService) // this won't work with decorators
                {
                    filesystemService.Dispose();
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
