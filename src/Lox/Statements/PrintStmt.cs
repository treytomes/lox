using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record PrintStmt(Expr Expression) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitPrintStmt(this);
	}
}
