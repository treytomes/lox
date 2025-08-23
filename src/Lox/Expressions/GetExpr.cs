using Lox.Visitors;

namespace Lox.Expressions;

public record GetExpr(Expr Object, Token Name) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitGetExpr(this);
	}
}
