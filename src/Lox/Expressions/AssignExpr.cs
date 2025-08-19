namespace Lox.Expressions;

public record AssignExpr<T>(Token Name, Expr<T> Value) : Expr<T>
{
	public override T Accept(IVisitor<T> visitor)
	{
		return visitor.VisitAssignExpr(this);
	}
}
