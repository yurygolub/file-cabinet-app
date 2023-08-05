namespace FileCabinetApp.CommandHandlers
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return commandHandler;
        }

        public virtual AppCommandRequest Handle(AppCommandRequest request)
        {
            return this.nextHandler?.Handle(request);
        }
    }
}
