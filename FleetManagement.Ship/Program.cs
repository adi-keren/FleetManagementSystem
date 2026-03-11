using FleetManagement.Ship.Executors;
using FleetManagement.Common.Extensions;
using FleetManagement.Ship.Factories;
using FleetManagement.Ship.Interfaces;
using FleetManagement.Ship.Services;
using FleetManagement.Ship.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddFleetLogging();

builder.Services.AddSingleton<ICommandExecutor, ProcessExecutor>();
builder.Services.AddSingleton<ICommandExecutor, ReadFileExecutor>();
builder.Services.AddSingleton<ICommandExecutor, WriteFileExecutor>();
builder.Services.AddSingleton<ICommandExecutorFactory, CommandExecutorFactory>();
builder.Services.AddSingleton(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ShipSocketClient>>();
    var factory = sp.GetRequiredService<ICommandExecutorFactory>();
    var configuration = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
    var hubUrl = configuration["FleetHub:Url"] ?? throw new InvalidOperationException("FleetHub:Url configuration is required.");
    
    Console.WriteLine("Enter Ship ID:");
    var shipId = "ship_" + Console.ReadLine()?.Trim();
    Console.WriteLine($"Registered Ship ID: {shipId}");
    return new ShipSocketClient(hubUrl, factory, logger, shipId);
});
builder.Services.AddHostedService<ShipWorker>();

var host = builder.Build();
await host.RunAsync();