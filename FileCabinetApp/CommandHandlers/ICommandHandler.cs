namespace FileCabinetApp.CommandHandlers
{
    public interface ICommandHandler
    {
        ICommandHandler SetNext(ICommandHandler commandHandler);

        AppCommandRequest Handle(AppCommandRequest request);
    }
}
