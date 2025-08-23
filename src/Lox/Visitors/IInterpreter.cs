using Lox.Expressions;

namespace Lox.Visitors;

public interface IInterpreter : IExprVisitor<object?>
{
	object? Evaluate(Expr expr);
}
