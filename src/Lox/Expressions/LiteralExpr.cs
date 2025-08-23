using Lox.Visitors;

namespace Lox.Expressions;

public record LiteralExpr(object? Value) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitLiteralExpr(this);
	}
}
