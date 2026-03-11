using FleetManagement.Common.Models;
using FleetManagement.Ship.Commands;
using FleetManagement.Ship.Interfaces;


namespace FleetManagement.Ship.Executors
{
    internal class WriteFileExecutor : ICommandExecutor
    {
        public bool CanHandle(ICommand command) => command is WriteFileCommand;

        public async Task<Response> ExecuteAsync(ICommand command)
        {
            var writeCommand = (WriteFileCommand)command;

            try
            {
                var directory = Path.GetDirectoryName(writeCommand.filePath);
                // ensure the directory exists before writing the file
                if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(writeCommand.filePath, writeCommand.content);

                    return new Response
                    {
                        RequstId = writeCommand.RequestId,
                        IsSuccess = true,
                        Output = $"Successfully wrote to {writeCommand.filePath}"
                    };
                }
                catch (Exception ex)
                {
                    return new Response { RequstId = writeCommand.RequestId, IsSuccess = false, ErrorMessage = ex.Message };
                }
        }
    }
}
