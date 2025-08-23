using Lox.Statements;

namespace Lox.Visitors;

public interface IStmtVisitor<T>
{
	T VisitBlockStmt(BlockStmt stmt);
	T VisitClassStmt(ClassStmt stmt);
	T VisitExpressionStmt(ExpressionStmt stmt);
	T VisitFunctionStmt(FunctionStmt stmt);
	T VisitIfStmt(IfStmt stmt);
	T VisitPrintStmt(PrintStmt stmt);
	T VisitReturnStmt(ReturnStmt stmt);
	T VisitVarStmt(VarStmt stmt);
	T VisitWhileStmt(WhileStmt stmt);
}
