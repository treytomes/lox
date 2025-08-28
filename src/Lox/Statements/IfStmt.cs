using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record IfStmt(Expr Condition, Stmt ThenBranch, Stmt? ElseBranch) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitIfStmt(this);
	}
}
