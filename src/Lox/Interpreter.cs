using Microsoft.Extensions.Logging;

namespace Lox;

public class Interpreter : IInterpreter
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IScannerFactory _scannerFactory;
	private readonly ILogger<Interpreter> _logger;

	#endregion

	#region Constructors

	public Interpreter(IErrorReporter errorReporter, IScannerFactory scannerFactory, ILogger<Interpreter> logger)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (scannerFactory == null) throw new ArgumentNullException(nameof(scannerFactory));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_errorReporter = errorReporter;
		_scannerFactory = scannerFactory;
		_logger = logger;
	}

	#endregion

	#region Methods

	public async Task RunAsync(IExecutionSource source)
	{
		var sourceText = await source.GetSourceAsync();
		var scanner = _scannerFactory.GetScanner(sourceText);
		var tokens = scanner.ScanTokens(sourceText);
		foreach (var token in tokens)
		{
			Console.WriteLine(token);
		}
	}

	#endregion
}
