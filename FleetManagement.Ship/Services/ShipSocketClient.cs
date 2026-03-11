using FleetManagement.Common.Constants;
using FleetManagement.Common.Models;
using FleetManagement.Ship.Factories;
using FleetManagement.Ship.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Ship.Services
{
    internal class ShipSocketClient
    {
        private readonly HubConnection _connection;
        private readonly ICommandExecutorFactory _executorFactory;
        private readonly ILogger<ShipSocketClient> _logger;
        private readonly string _shipId;

        public ShipSocketClient(string hubUrl, ICommandExecutorFactory executorFactory, ILogger<ShipSocketClient> logger, 
        string shipId)
        {
            _executorFactory = executorFactory;
            _logger = logger;
            _shipId = shipId;
            // start building the connection to the hub with the provided URL and shipId as a query parameter
            _connection = new HubConnectionBuilder()
                .WithUrl($"{hubUrl}?shipId={_shipId}")
                .WithAutomaticReconnect() 
                .Build();

            // set up a handler for incoming requests from the hub
            _connection.On<Request>(HubConstants.ExecuteCommandMethod, async (request) =>
            {
                await HandleRequestAsync(request);
            });
        }

        public async Task StartAsync(CancellationToken ct)
        {
            try
            {
                await _connection.StartAsync(ct);
                _logger.LogInformation("Connected to HQ at {Url}", _connection.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to HQ.");
            }
        }

        private async Task HandleRequestAsync(Request request)
        {
            _logger.LogInformation("Received request {RequestId} of type {Type}", request.RequstId, request.Type);

            try
            {
                // use CommandExecutorFactory to create the appropriate command based on the request type
                ICommand command = CommandExecutorFactory.CreateCommand(request);
                _logger.LogInformation($"Received command of type {command.GetType().Name}: " +
                    $"{string.Join(" ", request.Parameters.Select(p => p.Value))}");

                // use CommandExecutorFactory to get the appropriate executor for the command
                ICommandExecutor executor = _executorFactory.GetExecutor(command);

                // execute the command and get the response
                Response response = await executor.ExecuteAsync(command);
                if (response.IsSuccess)
                {
                    _logger.LogInformation($"Command executed successfully. Output: {response.Output}");
                }
                else
                {
                    _logger.LogError($"Command execution failed. Error: {response.ErrorMessage}");
                }

                // send the response back to HQ
                await _connection.InvokeAsync(HubConstants.SendResponseMethod, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request {RequestId}", request.RequstId);
                // send an error response back to HQ
                await _connection.InvokeAsync(HubConstants.SendResponseMethod, new Response
                {
                    RequstId = request.RequstId,
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public async Task StopAsync() => await _connection.StopAsync();
    }
}
