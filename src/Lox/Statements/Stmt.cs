using Lox.Visitors;

namespace Lox.Statements;

public abstract record Stmt
{
	public abstract void Accept(IStmtVisitor visitor);
}
