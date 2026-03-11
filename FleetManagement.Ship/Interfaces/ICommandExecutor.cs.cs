
using FleetManagement.Common.Models;

namespace FleetManagement.Ship.Interfaces
{
    public interface ICommandExecutor
    {
        bool CanHandle(ICommand command);
        Task<Response> ExecuteAsync(ICommand command);
    }
}
