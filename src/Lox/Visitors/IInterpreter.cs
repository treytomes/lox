using Lox.Expressions;

namespace Lox.Visitors;

public interface IInterpreter : IExprVisitor<object?>, IStmtVisitor<object?>
{
	object? Evaluate(Expr expr);
}
