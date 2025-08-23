using Lox.Visitors;

namespace Lox.Expressions;

public record AssignExpr(Token Name, Expr Value) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitAssignExpr(this);
	}
}
