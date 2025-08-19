namespace Lox.Expressions;

public record LogicalExpr<T>(Expr<T> Left, Token Operator, Expr<T> Right) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitLogicalExpr(this);
	}
}
