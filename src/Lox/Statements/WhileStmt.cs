using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record WhileStmt(Expr Condition, Stmt Body) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitWhileStmt(this);
	}
}
