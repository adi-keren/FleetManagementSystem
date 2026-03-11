using FleetManagement.Common.Models;
using FleetManagement.Common.Constants;
using FleetManagement.HQ.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FleetManagement.HQ.Hubs
{
    public class FleetHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILogger<FleetHub> _logger;
        private readonly ICommandQueue _commandQueue;

        public FleetHub(IConnectionManager connectionManager, ILogger<FleetHub> logger, ICommandQueue commandQueue)
        {
            _connectionManager = connectionManager;
            _logger = logger;
            _commandQueue = commandQueue;
        }
        // called when a ship connects to the hub
        public override async Task OnConnectedAsync()
        {

            var shipId = Context.GetHttpContext()?.Request.Query["shipId"].ToString();
            // if shipId is not null or empty, add the connection to the connection manager and send any pending commands
            if (!string.IsNullOrEmpty(shipId))
            {
                _logger.LogInformation($"{shipId} connected with ConnectionId {Context.ConnectionId}");
                _connectionManager.AddConnection(shipId, Context.ConnectionId);
                // check if there are any pending commands for this ship and send them
                while (_commandQueue.TryDequeue(shipId, out var pendingRequest) && pendingRequest is not null)
                {
                    _logger.LogInformation($"Sending pending command {pendingRequest.RequstId} to {shipId}");
                    await Clients.Caller.SendAsync(HubConstants.ExecuteCommandMethod, pendingRequest);
                }
            }
            else
            {
                _logger.LogError($"Recived connection with no ShipId");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var shipId = _connectionManager.GetShipIdByConnectionId(Context.ConnectionId);
            // remove the connection from the connection manager when a ship disconnects
            _connectionManager.RemoveConnection(Context.ConnectionId);
            _logger.LogInformation($"{shipId} disconnected with ConnectionId {Context.ConnectionId}");
            // call the base method to complete the disconnection process
            await base.OnDisconnectedAsync(exception);
        }
        
        public void SendResponse(Response response)
        {
            _logger.LogInformation($"Received response for request {response.RequstId}: " +
                $"Success: {response.IsSuccess}; "
                );
            if (!response.IsSuccess)
            {
                _logger.LogError($"Error Message: {response.ErrorMessage}");
            }
            else
            {
                _logger.LogInformation($"Output: {response.Output}");
            }
        }
        
    }
}
