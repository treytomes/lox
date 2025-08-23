using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record IfStmt(Expr Condition, Stmt ThenBranch, Stmt ElseBranch) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitIfStmt(this);
	}
}
