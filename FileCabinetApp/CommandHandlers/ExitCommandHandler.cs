using System;
using FileCabinetApp.FileCabinetService;

namespace FileCabinetApp.CommandHandlers
{
    public class ExitCommandHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (request.Command == "exit")
            {
                Console.WriteLine("Exiting an application...");
                Program.isRunning = false;

                if (Program.fileCabinetService is FileCabinetFilesystemService filesystemService)
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
