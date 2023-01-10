using System;

namespace FileCabinetApp.CommandHandlers
{
    public class DefaultHandler : CommandHandlerBase
    {
        public override AppCommandRequest Handle(AppCommandRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            Console.WriteLine($"There is no '{request.Command}' command.");

            return request;
        }
    }
}
