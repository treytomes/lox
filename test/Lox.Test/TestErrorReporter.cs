using Lox.Reporting;

namespace Lox.Test;

public class TestErrorReporter : IErrorReporter
{
	#region Fields

	private bool _hadError = false;
	private List<LoxError> _errors = new();

	#endregion

	#region Properties

	public bool HadError => _hadError;
	public IReadOnlyList<LoxError> Errors => _errors.AsReadOnly();

	#endregion

	#region Methods

	public void Error(int line, string message)
	{
		Report(line, string.Empty, message);
	}

	public void Report(int line, string where, string message)
	{
		_errors.Add(new LoxError(line, where, message));
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
