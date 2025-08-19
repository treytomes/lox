namespace Lox.Reporting;

public record LoxError(int Line, string Where, string Message)
{
	public override string ToString()
	{
		return $"[line {Line}] Error{Where}: {Message}";
	}
}
