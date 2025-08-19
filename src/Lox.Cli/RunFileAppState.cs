using Lox.Reporting;
using Microsoft.Extensions.Logging;

namespace Lox.Cli;

public class RunFileAppState : IAppState
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IInterpreter _interpreter;
	private readonly IExecutionSource _source;
	private readonly ILogger<RunFileAppState> _logger;

	#endregion

	#region Constructors

	public RunFileAppState(IErrorReporter errorReporter, IInterpreter interpreter, IExecutionSource source, ILogger<RunFileAppState> logger)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_errorReporter = errorReporter;
		_interpreter = interpreter;
		_source = source;
		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync()
	{
		await _interpreter.RunAsync(_source);
		if (_errorReporter.HadError)
		{
			throw new ApplicationException("There was an error running your program.");
		}
	}

	#endregion
}
