namespace Lox.Expressions;

public abstract record Expr
{
	public abstract T Accept<T>(IVisitor<T> visitor);
}
