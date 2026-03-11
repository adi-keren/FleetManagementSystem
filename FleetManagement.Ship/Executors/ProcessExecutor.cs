using System.Diagnostics;
using System.Text;
using FleetManagement.Common.Models;
using FleetManagement.Ship.Commands;
using FleetManagement.Ship.Interfaces;


namespace FleetManagement.Ship.Executors
{
    internal class ProcessExecutor : ICommandExecutor
    {
        public bool CanHandle(ICommand command) => command is ExecuteProcessCommand;

        public async Task<Response> ExecuteAsync(ICommand command)
        {
            var processCommand = (ExecuteProcessCommand)command;
            try
            {
                // build the command to execute the process with arguments
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {processCommand._processName} {processCommand._arguments}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processStartInfo };

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        outputBuilder.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        errorBuilder.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                var exitCode = process.ExitCode;
                var output = outputBuilder.ToString();
                var error = errorBuilder.ToString();
                // prepare the response based on the exit code and output
                if (exitCode == 0)
                    {
                        return new Response
                        {
                            RequstId = processCommand.RequestId,
                            IsSuccess = true,
                            Output = $"Exit Code: {exitCode}\nStdout: {output}\nStderr: {error}"
                        };
                    }
                    else
                    {
                        return new Response
                        {
                            RequstId = processCommand.RequestId,
                            IsSuccess = false,
                            Output = output,
                            ErrorMessage = $"Process exited with code {exitCode}. Stderr: {error}"
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new Response
                    {
                        RequstId = processCommand.RequestId,
                        IsSuccess = false,
                        ErrorMessage = $"Failed to execute process: {ex.Message}"
                    };
            }
        }
    }
}
