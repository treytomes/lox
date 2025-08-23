using Lox.Visitors;

namespace Lox.Expressions;

public record VariableExpr(Token Name) : Expr
{
	public override T Accept<T>(IExprVisitor<T> visitor)
	{
		return visitor.VisitVariableExpr(this);
	}
}
