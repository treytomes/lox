using Lox.Visitors;

namespace Lox.Expressions;

public record LogicalExpr(Expr Left, Token Operator, Expr Right) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitLogicalExpr(this);
	}
}
