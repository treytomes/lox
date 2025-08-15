namespace Lox;

public interface IExecutionSource
{
	string GetSource();
	Task<string> GetSourceAsync();
}
