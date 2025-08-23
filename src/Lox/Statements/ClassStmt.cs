using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record ClassStmt(Token Name, VariableExpr SuperClass, IList<FunctionStmt> Methods) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitClassStmt(this);
	}
}
