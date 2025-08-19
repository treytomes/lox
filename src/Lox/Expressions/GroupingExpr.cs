namespace Lox.Expressions;

public record GroupingExpr(Expr Expression) : Expr
{
	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitGroupingExpr(this);
	}
}
