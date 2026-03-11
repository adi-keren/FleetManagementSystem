using FleetManagement.HQ.Interfaces;

namespace FleetManagement.HQ.Workers;

internal class HQConsoleWorker : BackgroundService
{
    private readonly IConnectionManager _connectionManager;
    private readonly IUserInteractor _userInteractor;
    private readonly ILogger<HQConsoleWorker> _logger;

    public HQConsoleWorker(IConnectionManager connectionManager, IUserInteractor userInteractor,
        ILogger<HQConsoleWorker> logger)
    {
        _connectionManager = connectionManager;
        _userInteractor = userInteractor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // to ensure the worker starts after the application is fully initialized
        await Task.Yield();

        _logger.LogInformation("HQ Console Worker is ready.");

        _ = Task.Run(async () =>
        {
            DisplayPrompt();
            while (!ct.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    DisplayPrompt();
                    continue;
                }
                await _userInteractor.ProcessInputAsync(input);
            }
        }, ct);
    }

    private void DisplayPrompt()
    {
        _logger.LogInformation("\n--- Fleet Command Center ---");
        _logger.LogInformation("Available ships: {Ships}", string.Join(", ", _connectionManager.GetOnlineShips()));
        _logger.LogInformation("Enter command (format: [ShipId] [Type] [Path/Process]):");
        _logger.LogInformation("Example: Ship_01 ReadFile C:\\temp\\test.txt");
    }
}