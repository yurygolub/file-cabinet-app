using System;
using FileCabinetApp.FileCabinetService;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : ServiceCommandHandlerBase
    {
        public ExitCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "exit")
            {
                Console.WriteLine("Exiting an application...");
                Program.isRunning = false;

                if (this.service is FileCabinetFilesystemService filesystemService)
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
