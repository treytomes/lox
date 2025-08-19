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

	public void Error(int line, string message)
	{
		Report(line, string.Empty, message);
	}

	public void Report(int line, string where, string message)
	{
		Console.WriteLine(new LoxError(line, where, message).ToString());
		_hadError = true;
	}

	public void Error(Token token, string message)
	{
		if (token.Type == TokenType.EOF)
		{
			Report(token.Line, " at end", message);
		}
		else
		{
			Report(token.Line, $" at '{token.Lexeme}'", message);
		}
	}

	public void ResetErrorFlag()
	{
		_hadError = false;
	}

	#endregion
}
