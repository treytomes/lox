using Lox.Visitors;

namespace Lox.Expressions;

public record SuperExpr(Token Keyword, Token Method) : Expr
{
	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitSuperExpr(this);
	}
}
