namespace Lox;

public interface ILox
{
	Task RunAsync(IExecutionSource source);
}
