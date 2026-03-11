using System.Collections.Concurrent;
using FleetManagement.HQ.Interfaces;

namespace FleetManagement.HQ.Services
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _onlineShips = new();

        public void AddConnection(string shipId, string connectionId)
        {
            // Add or update the connection for the given shipId
            _onlineShips.AddOrUpdate(shipId, connectionId, (key, oldVal) => connectionId);
        }

        public void RemoveConnection(string connectionId)
        {
            // find the shipId associated with the given connectionId and remove it from the dictionary
            var item = _onlineShips.FirstOrDefault(x => x.Value == connectionId);
            if (!item.Equals(default(KeyValuePair<string, string>)))
            {
                _onlineShips.TryRemove(item.Key, out _);
            }
        }

        public string? GetConnectionId(string shipId)
        {
            // get the connectionId for the given shipId
            return _onlineShips.TryGetValue(shipId, out var connectionId) ? connectionId : null;
        }
        public string? GetShipIdByConnectionId(string connectionId)
        {
            // get the shipId associated with the given connectionId
            var item = _onlineShips.FirstOrDefault(x => x.Value == connectionId);
            return !item.Equals(default(KeyValuePair<string, string>)) ? item.Key : null;
        }

        public IEnumerable<string> GetOnlineShips() => _onlineShips.Keys;
    }
}