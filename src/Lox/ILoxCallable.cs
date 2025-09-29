using Lox.Interpreting;

namespace Lox;

public interface ILoxCallable
{
	object? Call(IInterpreter interpreter, IList<object?> arguments);
}
