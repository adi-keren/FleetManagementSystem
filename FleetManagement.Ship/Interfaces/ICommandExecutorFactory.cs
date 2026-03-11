namespace FleetManagement.Ship.Interfaces
{
    public interface ICommandExecutorFactory
    {
        ICommandExecutor GetExecutor(ICommand command);
    }
}
