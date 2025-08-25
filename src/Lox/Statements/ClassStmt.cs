using Lox.Expressions;
using Lox.Visitors;

namespace Lox.Statements;

public record ClassStmt(Token Name, VariableExpr SuperClass, IList<FunctionStmt> Methods) : Stmt
{
	public override void Accept(IStmtVisitor visitor)
	{
		visitor.VisitClassStmt(this);
	}
}
