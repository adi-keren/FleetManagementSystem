using FleetManagement.Common.Models;
using FleetManagement.Ship.Commands;
using FleetManagement.Ship.Interfaces;


namespace FleetManagement.Ship.Executors
{
    internal class ReadFileExecutor : ICommandExecutor
    {
        public bool CanHandle(ICommand command) => command is ReadFileCommand;

        public async Task<Response> ExecuteAsync(ICommand command)
        {
            var readCommand = (ReadFileCommand)command;

            try
            {
                if (!File.Exists(readCommand.filePath))
                {
                    return new Response
                    {
                        RequstId = readCommand.RequestId,
                        IsSuccess = false,
                        ErrorMessage = $"File not found: {readCommand.filePath}"
                    };
                }

                var content = await File.ReadAllTextAsync(readCommand.filePath);
                    return new Response { RequstId = readCommand.RequestId, IsSuccess = true, Output = content };
                }
                catch (Exception ex)
                {
                    return new Response { RequstId = readCommand.RequestId, IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
