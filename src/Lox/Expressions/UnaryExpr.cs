namespace Lox.Expressions;

public record UnaryExpr<T>(Token Operator, Expr<T> Right) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitUnaryExpr(this);
	}
}
