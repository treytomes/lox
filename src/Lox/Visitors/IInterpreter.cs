using Lox.Expressions;

namespace Lox.Visitors;

public interface IInterpreter : IVisitor<object?>
{
	object? Evaluate(Expr expr);
}
