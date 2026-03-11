using FleetManagement.Common.Models;
using FleetManagement.Common.Constants;
using FleetManagement.Ship.Interfaces;

namespace FleetManagement.Ship.Commands
{
    public class ReadFileCommand : ICommand
    {
        public Guid RequestId { get; }
        public readonly string filePath;

        public ReadFileCommand(Request request)
        {
            RequestId = request.RequstId;
            if (!request.Parameters.TryGetValue(ParameterConstants.FilePath, out var path) || string.IsNullOrWhiteSpace(path))
                throw new ArgumentException($"{ParameterConstants.FilePath} parameter is required for ReadFile command.");
            filePath = path;
        }
    }
}
