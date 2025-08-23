using Lox.Visitors;

namespace Lox.Statements;

public record FunctionStmt(Token Name, IList<Token> Params, IList<Stmt> Body) : Stmt
{
	public override T Accept<T>(IStmtVisitor<T> visitor)
	{
		return visitor.VisitFunctionStmt(this);
	}
}
