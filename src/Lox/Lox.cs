using Lox.Exceptions;
using Lox.Parsing;
using Lox.Reporting;
using Lox.Scanning;
using Lox.Visitors;
using Microsoft.Extensions.Logging;

namespace Lox;

public class Lox : ILox
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IScanner _scanner;
	private readonly IParser _parser;
	private readonly IInterpreter _interpreter;
	private readonly ILogger<Lox> _logger;

	#endregion

	#region Constructors

	public Lox(IScanner scanner, IParser parser, IInterpreter interpreter, IErrorReporter errorReporter, ILogger<Lox> logger)
	{
		if (scanner == null) throw new ArgumentNullException(nameof(scanner));
		if (parser == null) throw new ArgumentNullException(nameof(parser));
		if (interpreter == null) throw new ArgumentNullException(nameof(interpreter));
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (logger == null) throw new ArgumentNullException(nameof(logger));

		_scanner = scanner;
		_parser = parser;
		_interpreter = interpreter;
		_errorReporter = errorReporter;
		_logger = logger;
	}

	#endregion

	#region Properties

	public object? LastResult => _interpreter.LastResult;

	#endregion

	#region Methods

	public async Task RunAsync(IExecutionSource source)
	{
		var sourceText = await source.GetSourceAsync();
		var tokens = _scanner.ScanTokens(sourceText);
		var stmts = _parser.Parse(tokens);
		if (stmts != null)
		{
			try
			{
				_interpreter.Interpret(stmts);
			}
			catch (RuntimeException ex)
			{
				_errorReporter.RuntimeError(ex);
			}
		}
	}

	public void ResetLastResult()
	{
		_interpreter.ResetLastResult();
	}

	#endregion
}
