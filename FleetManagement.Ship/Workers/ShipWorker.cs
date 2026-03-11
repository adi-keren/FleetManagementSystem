using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FleetManagement.Ship.Services;

namespace FleetManagement.Ship.Workers
{
    internal class ShipWorker : BackgroundService
    {
        private readonly ShipSocketClient _socketClient;
        private readonly ILogger<ShipWorker> _logger;

        public ShipWorker(ShipSocketClient socketClient, ILogger<ShipWorker> logger)
        {
            _socketClient = socketClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Ship Worker is starting...");

            // connect to the HQ server and start listening for commands
            await _socketClient.StartAsync(ct);

            _logger.LogInformation("Ship is connected and listening for commands.");

            // Keep the worker running until cancellation is requested.
            await Task.Delay(Timeout.Infinite, ct);

            _logger.LogInformation("Ship Worker is stopping, closing connection...");
            await _socketClient.StopAsync();
        }
    }
}