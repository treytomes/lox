using Lox.Expressions;
using Lox.Statements;

namespace Lox.Visitors;

public interface IInterpreter : IExprVisitor<object?>, IStmtVisitor
{
	object? Evaluate(Expr expr);
	void Interpret(IList<Stmt> statements);
}
