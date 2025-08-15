using Microsoft.Extensions.Logging;

namespace Lox.Cli;

public class RunPromptAppState : IAppState
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IInterpreter _interpreter;
	private readonly ILogger<RunFileAppState> _logger;

	#endregion

	#region Constructors

	public RunPromptAppState(IErrorReporter errorReporter, IInterpreter interpreter, ILogger<RunFileAppState> logger)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_errorReporter = errorReporter;
		_interpreter = interpreter;
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
			await _interpreter.RunAsync(new ImmediateExecutionSource(sourceText));
			_errorReporter.ResetErrorFlag();
		}
	}

	#endregion
}
