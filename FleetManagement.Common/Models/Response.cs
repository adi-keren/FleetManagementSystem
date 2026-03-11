
namespace FleetManagement.Common.Models
{
    public class Response
    {
        public Guid RequstId { get; set; }

        public bool IsSuccess { get; set; }

        public string Output { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
