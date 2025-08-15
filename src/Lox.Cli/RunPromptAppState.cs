using Microsoft.Extensions.Logging;

namespace Lox.Cli;

public class RunPromptAppState : IAppState
{
	#region Fields

	private readonly ILogger<RunFileAppState> _logger;

	#endregion

	#region Constructors

	public RunPromptAppState(ILogger<RunFileAppState> logger)
	{
		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync()
	{
		await Task.Yield();

		_logger.LogInformation("Running interpreter prompt.");

		_logger.LogInformation("Not implemented yet.");
	}

	private async void RunFile(string path)
	{
		var text = await File.ReadAllTextAsync(path);

		Console.WriteLine(text);
	}

	#endregion
}
