using Microsoft.Extensions.Logging;

namespace Lox;

public class Interpreter : IInterpreter
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IScanner _scanner;
	private readonly ILogger<Interpreter> _logger;

	#endregion

	#region Constructors

	public Interpreter(IErrorReporter errorReporter, IScanner scanner, ILogger<Interpreter> logger)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (scanner == null) throw new ArgumentNullException(nameof(scanner));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_errorReporter = errorReporter;
		_scanner = scanner;
		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync(IExecutionSource source)
	{
		var sourceText = await source.GetSourceAsync();
		var tokens = _scanner.ScanTokens(sourceText);
		foreach (var token in tokens)
		{
			Console.WriteLine(token);
		}
	}

	#endregion
}
