using Lox.Visitors;

namespace Lox.Expressions;

public record SetExpr(Expr Object, Token Name, Expr Value) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitSetExpr(this);
	}
}
