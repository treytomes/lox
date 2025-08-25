using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record WhileStmt(Expr Condition, Stmt Body) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitWhileStmt(this);
	}
}
