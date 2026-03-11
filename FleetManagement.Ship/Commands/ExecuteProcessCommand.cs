using FleetManagement.Common.Constants;
using FleetManagement.Common.Models;
using FleetManagement.Ship.Interfaces;

namespace FleetManagement.Ship.Commands
{
    public class ExecuteProcessCommand : ICommand
    {
        public Guid RequestId { get; }
        public readonly string _processName;
        public readonly string _arguments;

        public ExecuteProcessCommand(Request request)
        {
            RequestId = request.RequstId;
            if (!request.Parameters.TryGetValue(ParameterConstants.ProcessName, out var processName) || string.IsNullOrWhiteSpace(processName))
                throw new ArgumentException($"{ParameterConstants.ProcessName} parameter is required for RunProcess command.");

            _processName = processName;

            request.Parameters.TryGetValue(ParameterConstants.Arguments, out var arguments);
            _arguments = arguments ?? string.Empty;
        }
    }
}
