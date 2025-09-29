using Lox.Exceptions;

namespace Lox.Reporting;

public class ConsoleErrorReporter : IErrorReporter
{
	#region Fields

	private bool _hadError = false;
	private bool _hadRuntimeError = false;

	#endregion

	#region Properties

	public bool HadError => _hadError;
	public bool HadRuntimeError => _hadRuntimeError;

	#endregion

	#region Methods

	public void Report(int line, string where, string message)
	{
		Console.WriteLine(new LoxError(line, where, message).ToString());
		_hadError = true;
	}

	public void RuntimeError(RuntimeException error)
	{
		var line = error.Token.Line;
		Console.WriteLine($"{error.Message}\n[line {line}]");
		_hadRuntimeError = true;
	}

	public void ResetErrorFlags()
	{
		_hadError = false;
		_hadRuntimeError = false;
	}

	#endregion
}
