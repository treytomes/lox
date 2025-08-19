namespace Lox.Expressions;

public record GetExpr<T>(Expr<T> Object, Token Name) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitGetExpr(this);
	}
}
