namespace FleetManagement.Ship.Interfaces
{
    public interface ICommand
    {
        Guid RequestId { get; }
    }
}
