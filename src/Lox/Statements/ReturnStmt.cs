using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record ReturnStmt(Token Keyword, Expr Value) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitReturnStmt(this);
	}
}
