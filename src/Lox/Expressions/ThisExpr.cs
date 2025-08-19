namespace Lox.Expressions;

public record ThisExpr<T>(Token Keyword) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitThisExpr(this);
	}
}
