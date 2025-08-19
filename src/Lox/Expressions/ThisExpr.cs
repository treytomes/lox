namespace Lox.Expressions;

public record ThisExpr(Token Keyword) : Expr
{
	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitThisExpr(this);
	}
}
