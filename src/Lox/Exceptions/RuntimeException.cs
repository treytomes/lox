namespace Lox.Exceptions;

public class RuntimeException : LoxException
{
	public RuntimeException(Token? token, string message)
		: base(message)
	{
		Token = token;
	}

	public Token? Token { get; }
}
