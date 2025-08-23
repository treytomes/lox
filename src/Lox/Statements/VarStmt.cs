using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record VarStmt(Token Name, Expr Initializer) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitVarStmt(this);
	}
}
