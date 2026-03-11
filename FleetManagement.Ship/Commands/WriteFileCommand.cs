using FleetManagement.Common.Models;
using FleetManagement.Common.Constants;
using FleetManagement.Ship.Interfaces;

namespace FleetManagement.Ship.Commands
{
    public class WriteFileCommand : ICommand
    {
        public Guid RequestId { get; }
        public readonly string filePath;
        public readonly string content;

        public WriteFileCommand(Request request)
        {
            RequestId = request.RequstId;
            filePath = request.Parameters.TryGetValue(ParameterConstants.FilePath, out var path) && !string.IsNullOrWhiteSpace(path)
                ? path
                : throw new ArgumentException($"{ParameterConstants.FilePath} parameter is required for WriteFile command.");
            content = request.Parameters.TryGetValue(ParameterConstants.Content, out var contentValue) ? contentValue ?? string.Empty : string.Empty;
        }
    }
}
