
using FleetManagement.Common.Models;

namespace FleetManagement.HQ.Interfaces
{
    public interface ICommandQueue
    {
        void Enqueue(string shipId, Request request);
        bool TryDequeue(string shipId, out Request? request);
    }
}
