namespace Lox;

public interface IInterpreter
{
	Task RunAsync(IExecutionSource source);
}
