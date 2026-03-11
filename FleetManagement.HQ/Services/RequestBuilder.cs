using FleetManagement.Common.Constants;
using FleetManagement.Common.Enums;
using FleetManagement.Common.Models;
using FleetManagement.HQ.Interfaces;

namespace FleetManagement.HQ.Services;

public class RequestBuilder : IRequestBuilder
{
    public Request Build(string shipId, CommandType commandType, string parametersStr)
    {
        // create a new Request object with the provided shipId, commandType, and parameters
        var request = new Request
        {
            RequstId = Guid.NewGuid(),
            Type = commandType,
            TargetShipId = shipId,
            Parameters = new Dictionary<string, string>()
        };
        // parse the parametersStr based on the commandType and populate the Parameters dictionary accordingly
        switch (commandType)
        {
            case CommandType.RunProcess:
                BuildRunProcessParameters(request, parametersStr);
                break;

            case CommandType.ReadFile:
                BuildReadFileParameters(request, parametersStr);
                break;

            case CommandType.WriteFile:
                BuildWriteFileParameters(request, parametersStr);
                break;

            default:
                throw new ArgumentException($"Unsupported command type: {commandType}");
        }

        return request;
    }

    private static void BuildRunProcessParameters(Request request, string parametersStr)
    {
        // Format: processName [arguments]
        // Example: "cmd.exe /c echo Hello World"
        var parts = parametersStr.Split(' ', 2);

        if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
        {
            throw new ArgumentException("ProcessName is required for RunProcess command.");
        }

        request.Parameters[ParameterConstants.ProcessName] = parts[0];
        request.Parameters[ParameterConstants.Arguments] = parts.Length > 1 ? parts[1] : string.Empty;
    }

    private static void BuildReadFileParameters(Request request, string parametersStr)
    {
        // Format: filePath
        // Example: "C:\temp\test.txt"
        if (string.IsNullOrWhiteSpace(parametersStr))
        {
            throw new ArgumentException("FilePath is required for ReadFile command.");
        }

        request.Parameters[ParameterConstants.FilePath] = parametersStr.Trim();
    }

    private static void BuildWriteFileParameters(Request request, string parametersStr)
    {
        // Format: filePath content
        // Example: "C:\temp\output.txt Hello World"
        var parts = parametersStr.Split(' ', 2);

        if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0]))
        {
            throw new ArgumentException("FilePath and Content are required for WriteFile command. Format: [FilePath] [Content]");
        }

        request.Parameters[ParameterConstants.FilePath] = parts[0];
        request.Parameters[ParameterConstants.Content] = parts[1];
    }
}
