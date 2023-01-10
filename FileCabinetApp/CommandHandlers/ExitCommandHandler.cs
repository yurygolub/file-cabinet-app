using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        public ExitCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "exit")
            {
                Console.WriteLine("Exiting an application...");
                Program.isRunning = false;

                if (this.fileCabinetService is FileCabinetFilesystemService filesystemService)
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
