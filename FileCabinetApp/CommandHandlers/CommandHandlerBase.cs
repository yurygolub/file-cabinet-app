using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            throw new NotImplementedException();
        }

        public abstract void Handle(AppCommandRequest request);
    }
}
