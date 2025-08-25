using Lox.Visitors;

namespace Lox.Statements;

public record FunctionStmt(Token Name, IList<Token> Params, IList<Stmt> Body) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitFunctionStmt(this);
	}
}
