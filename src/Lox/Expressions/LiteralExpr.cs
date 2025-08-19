namespace Lox.Expressions;

public record LiteralExpr<T>(object? Value) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitLiteralExpr(this);
	}
}
