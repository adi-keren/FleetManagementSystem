using FleetManagement.HQ.Hubs;
using FleetManagement.Common.Extensions;
using FleetManagement.HQ.Services;
using FleetManagement.HQ.Workers;
using FleetManagement.HQ.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddFleetLogging();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddSingleton<ICommandQueue, CommandQueue>();
builder.Services.AddSingleton<IRequestBuilder, RequestBuilder>();
builder.Services.AddSingleton<IUserInteractor, UserInteractor>();

builder.Services.AddHostedService<HQConsoleWorker>();

var app = builder.Build();
app.MapHub<FleetHub>("/fleetHub");

app.MapGet("/", () => "HQ Server is running!");

app.Run();