using FleetManagement.Common.Enums;
using FleetManagement.Common.Constants;
using FleetManagement.HQ.Hubs;
using FleetManagement.HQ.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FleetManagement.HQ.Services;

public class UserInteractor : IUserInteractor
{
    private readonly IHubContext<FleetHub> _hubContext;
    private readonly IConnectionManager _connectionManager;
    private readonly ICommandQueue _commandQueue;
    private readonly IRequestBuilder _requestBuilder;
    private readonly ILogger<UserInteractor> _logger;

    public UserInteractor(IHubContext<FleetHub> hubContext, IConnectionManager connectionManager,
        ICommandQueue commandQueue, IRequestBuilder requestBuilder, ILogger<UserInteractor> logger)
    {
        _hubContext = hubContext;
        _connectionManager = connectionManager;
        _commandQueue = commandQueue;
        _requestBuilder = requestBuilder;
        _logger = logger;
    }

    public async Task ProcessInputAsync(string input)
    {
        // Expected input format: [ShipId] [Type] [Parameters]
        try
        {
            var parts = input.Split(' ', 3);
            if (parts.Length < 3)
            {
                LogUsageInstructions();
                return;
            }

            var shipId = parts[0];
            var typeStr = parts[1];
            var parametersStr = parts[2];
            // validate command type
            if (!Enum.TryParse<CommandType>(typeStr, true, out var commandType))
            {
                _logger.LogError($"Invalid command type: {typeStr}. Valid types: RunProcess, ReadFile, WriteFile");
                return;
            }
            // Build request
            var request = _requestBuilder.Build(shipId, commandType, parametersStr);
            var connectionId = _connectionManager.GetConnectionId(shipId);
            // If the ship is not currently connected, queue the command for later delivery
            if (connectionId == null)
            {
                _logger.LogWarning($"{shipId} is offline. Queueing command {request.RequstId}...");
                _commandQueue.Enqueue(shipId, request);
                return;
            }
            // send command to the ship
            await _hubContext.Clients.Client(connectionId).SendAsync(HubConstants.ExecuteCommandMethod, request);
            _logger.LogInformation($"Command {request.RequstId} sent to {shipId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user input.");
        }
    }

    private void LogUsageInstructions()
    {
        _logger.LogWarning("Invalid format. Use: [ShipId] [Type] [Parameters]");
        _logger.LogInformation("Examples:");
        _logger.LogInformation("  Ship_01 RunProcess cmd.exe /c echo Hello");
        _logger.LogInformation("  Ship_01 ReadFile C:\\temp\\test.txt");
        _logger.LogInformation("  Ship_01 WriteFile C:\\temp\\output.txt Hello World");
    }
}
