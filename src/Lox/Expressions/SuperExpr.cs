namespace Lox.Expressions;

public record SuperExpr<T>(Token Keyword, Token Method) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitSuperExpr(this);
	}
}
