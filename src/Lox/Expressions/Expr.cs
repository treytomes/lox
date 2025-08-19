namespace Lox.Expressions;

public abstract record Expr<T>
{
	public abstract T Accept(IVisitor<T> visitor);
}
