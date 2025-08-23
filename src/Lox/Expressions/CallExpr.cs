using Lox.Visitors;

namespace Lox.Expressions;

public record CallExpr(Expr Callee, Token Paren, List<Expr> Arguments) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitCallExpr(this);
	}
}
