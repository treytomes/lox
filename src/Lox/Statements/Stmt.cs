using Lox.Visitors;

namespace Lox.Statements;

public abstract record Stmt
{
	public abstract T Accept<T>(IStmtVisitor<T> visitor);
}
