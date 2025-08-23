using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record PrintStmt(Expr Expression) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitPrintStmt(this);
	}
}
