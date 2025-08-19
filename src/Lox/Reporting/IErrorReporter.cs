using Lox.Exceptions;

namespace Lox.Reporting;

public interface IErrorReporter
{
	#region Properties

	bool HadError { get; }
	bool HadRuntimeError { get; }

	#endregion

	#region Methods

	void ResetErrorFlags();
	void Report(int line, string where, string message);
	void RuntimeError(RuntimeException error);

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
