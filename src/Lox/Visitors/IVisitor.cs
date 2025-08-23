using Lox.Expressions;

namespace Lox.Visitors;

public interface IVisitor<T>
{
	T VisitAssignExpr(AssignExpr expr);
	T VisitBinaryExpr(BinaryExpr expr);
	T VisitCallExpr(CallExpr expr);
	T VisitGetExpr(GetExpr expr);
	T VisitGroupingExpr(GroupingExpr expr);
	T VisitLiteralExpr(LiteralExpr expr);
	T VisitLogicalExpr(LogicalExpr expr);
	T VisitSetExpr(SetExpr expr);
	T VisitSuperExpr(SuperExpr expr);
	T VisitThisExpr(ThisExpr expr);
	T VisitUnaryExpr(UnaryExpr expr);
	T VisitVariableExpr(VariableExpr expr);
}
