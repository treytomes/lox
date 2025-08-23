using Lox.Visitors;

namespace Lox.Expressions;

public record GroupingExpr(Expr Expression) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitGroupingExpr(this);
	}
}
