namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        ICommandHandler SetNext(ICommandHandler commandHandler);

        void Handle(AppCommandRequest request);
    }
}
