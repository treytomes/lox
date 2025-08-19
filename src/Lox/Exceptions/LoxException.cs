namespace Lox.Exceptions;

public abstract class LoxException : ApplicationException
{
	public LoxException(string message = "There was an error.")
		: base(message)
	{
	}
}
