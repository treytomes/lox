namespace Lox.Reporting;

public class ConsoleErrorReporter : IErrorReporter
{
	#region Fields

	private bool _hadError = false;

	#endregion

	#region Properties

	public bool HadError => _hadError;

	#endregion

	#region Methods

	public void Report(int line, string where, string message)
	{
		Console.WriteLine(new LoxError(line, where, message).ToString());
		_hadError = true;
	}

	public void ResetErrorFlag()
	{
		_hadError = false;
	}

	#endregion
}
