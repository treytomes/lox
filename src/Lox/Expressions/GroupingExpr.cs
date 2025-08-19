namespace Lox.Expressions;

public record GroupingExpr<T>(Expr<T> Expression) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitGroupingExpr(this);
	}
}
