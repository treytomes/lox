using Lox.Visitors;

namespace Lox.Statements;

public record BlockStmt(IList<Stmt> Statements) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitBlockStmt(this);
	}
}
