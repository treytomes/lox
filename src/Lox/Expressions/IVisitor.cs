namespace Lox.Expressions;

public interface IVisitor<T>
{
	T VisitAssignExpr(AssignExpr<T> expr);
	T VisitBinaryExpr(BinaryExpr<T> expr);
	T VisitCallExpr(CallExpr<T> expr);
	T VisitGetExpr(GetExpr<T> expr);
	T VisitGroupingExpr(GroupingExpr<T> expr);
	T VisitLiteralExpr(LiteralExpr<T> expr);
	T VisitLogicalExpr(LogicalExpr<T> expr);
	T VisitSetExpr(SetExpr<T> expr);
	T VisitSuperExpr(SuperExpr<T> expr);
	T VisitThisExpr(ThisExpr<T> expr);
	T VisitUnaryExpr(UnaryExpr<T> expr);
	T VisitVariableExpr(VariableExpr<T> expr);
}
