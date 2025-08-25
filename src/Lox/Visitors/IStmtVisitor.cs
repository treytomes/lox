using Lox.Statements;

namespace Lox.Visitors;

public interface IStmtVisitor
{
	void VisitBlockStmt(BlockStmt stmt);
	void VisitClassStmt(ClassStmt stmt);
	void VisitExpressionStmt(ExpressionStmt stmt);
	void VisitFunctionStmt(FunctionStmt stmt);
	void VisitIfStmt(IfStmt stmt);
	void VisitPrintStmt(PrintStmt stmt);
	void VisitReturnStmt(ReturnStmt stmt);
	void VisitVarStmt(VarStmt stmt);
	void VisitWhileStmt(WhileStmt stmt);
}
