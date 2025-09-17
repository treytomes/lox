namespace Lox;

public interface ILox
{
	object? LastResult { get; }
	Task RunAsync(IExecutionSource source);
}
