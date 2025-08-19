using Lox.Reporting;
using Microsoft.Extensions.Logging;

namespace Lox.Cli;

public class RunFileAppState : IAppState
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly ILox _lox;
	private readonly IExecutionSource _source;
	private readonly ILogger<RunFileAppState> _logger;

	#endregion

	#region Constructors

	public RunFileAppState(IErrorReporter errorReporter, ILox lox, IExecutionSource source, ILogger<RunFileAppState> logger)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (lox == null) throw new ArgumentNullException(nameof(lox));
		if (source == null) throw new ArgumentNullException(nameof(source));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_errorReporter = errorReporter;
		_lox = lox;
		_source = source;
		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync()
	{
		await _lox.RunAsync(_source);
		if (_errorReporter.HadError)
		{
			throw new ApplicationException("There was an error running your program.");
		}
	}

	#endregion
}
