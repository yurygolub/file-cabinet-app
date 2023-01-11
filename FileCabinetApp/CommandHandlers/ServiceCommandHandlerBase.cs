using System;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        protected readonly IFileCabinetService service;

        public ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService ?? throw new ArgumentNullException(nameof(fileCabinetService));
        }
    }
}
