using Lox.Visitors;

namespace Lox.Expressions;

public record ThisExpr(Token Keyword) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitThisExpr(this);
	}
}
