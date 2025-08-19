namespace Lox.Expressions;

public record SetExpr<T>(Expr<T> Object, Token Name, Expr<T> Value) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitSetExpr(this);
	}
}
