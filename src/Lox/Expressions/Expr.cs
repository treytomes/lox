using Lox.Visitors;

namespace Lox.Expressions;

public abstract record Expr
{
	public abstract T Accept<T>(IExprVisitor<T> visitor);
}
