namespace Lox.Reporting;

public interface IErrorReporter
{
	#region Properties

	bool HadError { get; }

	#endregion

	#region Methods

	void ResetErrorFlag();
	void Report(int line, string where, string message);

	void Error(Token token, string message)
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

	void Error(int line, string message)
	{
		Report(line, string.Empty, message);
	}

	#endregion
}
