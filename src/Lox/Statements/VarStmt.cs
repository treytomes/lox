using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record VarStmt(Token Name, Expr Initializer) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitVarStmt(this);
	}
}
