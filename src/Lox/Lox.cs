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

	#region Methods

	public async Task RunAsync(IExecutionSource source)
	{
		var sourceText = await source.GetSourceAsync();
		var tokens = _scanner.ScanTokens(sourceText);
		var expr = _parser.Parse(tokens);
		if (expr != null)
		{
			try
			{
				var value = _interpreter.Evaluate(expr);
				Console.WriteLine(Stringify(value));
			}
			catch (RuntimeException ex)
			{
				_errorReporter.RuntimeError(ex);
			}
		}
	}

	private string Stringify(object? obj)
	{
		if (obj == null) return "nil";

		if (obj is double)
		{
			var text = obj.ToString();
			if (text == null) throw new NullReferenceException("This shouldn't have happened.");
			if (text.EndsWith(".0"))
			{
				text = text.Substring(0, text.Length - 2);
			}
			return text;
		}
		else if (obj is string)
		{
			return $"\"{obj}\"";
		}

		return obj.ToString() ?? string.Empty;
	}

	#endregion
}
