using Lox.Visitors;

namespace Lox.Statements;

public record BlockStmt(IList<Stmt> Statements) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitBlockStmt(this);
	}
}
