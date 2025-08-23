using Lox.Visitors;

namespace Lox.Expressions;

public record UnaryExpr(Token Operator, Expr Right) : Expr
{
	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitUnaryExpr(this);
	}
}
