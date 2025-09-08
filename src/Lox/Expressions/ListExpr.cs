using Lox.Visitors;

namespace Lox.Expressions;

public record ListExpr(Expr Left, Expr Right) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitListExpr(this);
	}
}
