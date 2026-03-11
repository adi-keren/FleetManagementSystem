using FleetManagement.Common.Enums;
using FleetManagement.Common.Models;
using FleetManagement.Ship.Commands;
using FleetManagement.Ship.Interfaces;

namespace FleetManagement.Ship.Factories
{
    internal class CommandExecutorFactory : ICommandExecutorFactory
    {
        private readonly IEnumerable<ICommandExecutor> _executors;

        public CommandExecutorFactory(IEnumerable<ICommandExecutor> executors)
        {
            _executors = executors;
        }

        public ICommandExecutor GetExecutor(ICommand command)
        {
            foreach (var executor in _executors)
            {
                if (executor.CanHandle(command))
                {
                    return executor;
                }
            }

            throw new InvalidOperationException($"No executor found for command type: {command.GetType().Name}");
        }

        public static ICommand CreateCommand(Request request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.Type switch
            {
                CommandType.RunProcess => new ExecuteProcessCommand(request),
                CommandType.ReadFile => new ReadFileCommand(request),
                CommandType.WriteFile => new WriteFileCommand(request),
                _ => throw new ArgumentException($"Unknown command type: {request.Type}", nameof(request))
            };
        }
    }
}
    