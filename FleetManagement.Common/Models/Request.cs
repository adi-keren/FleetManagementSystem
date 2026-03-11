using FleetManagement.Common.Enums;

namespace FleetManagement.Common.Models
{
    public class Request
    {
        public Guid RequstId { get; set; } = Guid.NewGuid();

        public CommandType Type { get; set; }

        public string TargetShipId { get; set; } = string.Empty;

        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
