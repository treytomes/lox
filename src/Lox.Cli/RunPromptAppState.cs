using Lox.Reporting;
using Microsoft.Extensions.Logging;

namespace Lox.Cli;

public class RunPromptAppState : IAppState
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly ILox _lox;
	private readonly ILogger<RunFileAppState> _logger;

	#endregion

	#region Constructors

	public RunPromptAppState(IErrorReporter errorReporter, ILox lox, ILogger<RunFileAppState> logger)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (lox == null) throw new ArgumentNullException(nameof(lox));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_errorReporter = errorReporter;
		_lox = lox;
		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync()
	{
		while (true)
		{
			Console.Write("> ");
			var sourceText = Console.ReadLine();
			if (sourceText == null)
			{
				break;
			}
			await _lox.RunAsync(new ImmediateExecutionSource(sourceText));
			if (_lox.LastResult != null)
			{
				Console.WriteLine(_lox.LastResult);
			}
			_lox.ResetLastResult();
			_errorReporter.ResetErrorFlags();
		}
	}

	#endregion
}
