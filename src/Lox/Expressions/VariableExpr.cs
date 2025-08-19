namespace Lox.Expressions;

public record VariableExpr<T>(Token Name) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitVariableExpr(this);
	}
}
