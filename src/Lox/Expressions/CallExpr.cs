namespace Lox.Expressions;

public record CallExpr<T>(Expr<T> Callee, Token Paren, List<Expr<T>> Arguments) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitCallExpr(this);
	}
}
