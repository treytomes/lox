using Microsoft.Extensions.Logging;

namespace Lox;

public class Interpreter : IInterpreter
{
	#region Fields

	private readonly ILogger<Interpreter> _logger;

	#endregion

	#region Constructors

	public Interpreter(ILogger<Interpreter> logger)
	{
		if (logger == null) throw new ArgumentNullException();

		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync(IExecutionSource source)
	{
		var sourceText = await source.GetSourceAsync();
		_logger.LogInformation("Running source: {sourceText}", sourceText);
	}

	#endregion
}
