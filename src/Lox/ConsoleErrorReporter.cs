namespace Lox;

public class ConsoleErrorReporter : IErrorReporter
{
	#region Fields

	private bool _hadError = false;

	#endregion

	#region Properties

	public bool HadError => _hadError;

	#endregion

	#region Methods

	public void Error(int line, string message)
	{
		Report(line, string.Empty, message);
	}

	public void Report(int line, string where, string message)
	{
		Console.WriteLine($"[line {line}] Error{where}: {message}");
		_hadError = true;
	}

	public void ResetErrorFlag()
	{
		_hadError = false;
	}

	#endregion
}
