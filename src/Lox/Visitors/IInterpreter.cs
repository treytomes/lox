using Lox.Expressions;

namespace Lox.Visitors;

public interface IInterpreter : IExprVisitor<object?>
{
	IEnvironment CurrentEnvironment { get; }
	object? LastResult { get; }

	object? Evaluate(Expr expr);
	void Interpret(IList<Stmt> statements);
}
