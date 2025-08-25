using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record ReturnStmt(Token Keyword, Expr Value) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitReturnStmt(this);
	}
}
