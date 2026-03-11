# Fleet Management System

A distributed fleet management system built with .NET 8 that enables real-time communication between a headquarters (HQ) server and multiple ship clients using SignalR.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows OS (for process execution commands)

## Setup and Run

### 1. Build the Solution

```bash
dotnet build
```

### 2. Start the Server (HQ)

Open a terminal and run:

```bash
cd FleetManagement.HQ
dotnet run
```

The server will start on `http://localhost:5000`.

### 3. Start the Client (Ship)

Open a **new terminal** and run:

```bash
cd FleetManagement.Ship
dotnet run
```

When prompted, enter a Ship ID (e.g., `01`). The ship will register as `ship_01`.

You can start multiple ship clients with different IDs.

## Additional Information

### Command Format

From the HQ console, send commands to ships using:

```
[ShipId] [CommandType] [Parameters]
```

### Supported Commands

| Command Type | Description | Example |
|--------------|-------------|---------|
| `RunProcess` | Execute a process | `ship_01 RunProcess ping localhost` |
| `ReadFile` | Read a file | `ship_01 ReadFile C:\temp\test.txt` |
| `WriteFile` | Write to a file | `ship_01 WriteFile C:\temp\output.txt Hello World` |

