
namespace FleetManagement.HQ.Interfaces
{
    public interface IConnectionManager
    {
        void AddConnection(string shipId, string connectionId);
        void RemoveConnection(string connectionId);
        string? GetConnectionId(string shipId);
        string? GetShipIdByConnectionId(string connectionId);
        IEnumerable<string> GetOnlineShips();
    }
}
