using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record ExpressionStmt(Expr Expression) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitExpressionStmt(this);
	}
}
