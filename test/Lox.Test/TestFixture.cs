
using Lox.Interpreting;
using Lox.Parsing;
using Lox.Reporting;
using Lox.Scanning;
using Lox.Test;

/// <summary>
/// Everything you need to spin up a Lox text.
/// </summary>
class TestFixture
{
	#region Fields

	private readonly TestErrorReporter _errorReporter = new TestErrorReporter();
	private readonly IScannerCursor _scannerCursor = new ScannerCursor();
	private readonly IParserCursor _parserCursor = new ParserCursor();
	private readonly TestOutputWriter _writer = new();
	private readonly IScanner _scanner;
	private readonly IParser _parser;
	private readonly IInterpreter _interpreter;

	#endregion

	#region Constructors

	public TestFixture()
	{
		_scanner = new Scanner(_scannerCursor, _errorReporter);
		_parser = new Parser(_parserCursor, _errorReporter);
		_interpreter = new Interpreter(_writer, _errorReporter);
	}

	#endregion

	#region Properties

	public TestErrorReporter ErrorReporter => _errorReporter;
	public IScanner Scanner => _scanner;
	public IParser Parser => _parser;
	public TestOutputWriter Writer => _writer;
	public IInterpreter Interpreter => _interpreter;

	#endregion
}
