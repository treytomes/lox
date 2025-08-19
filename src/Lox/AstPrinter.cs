using System.Text;
using Lox.Expressions;

namespace Lox;

public class AstPrinter : IVisitor<string>
{
	public string ToString(Expr<string> expr)
	{
		return expr.Accept(this);
	}

	public string VisitAssignExpr(AssignExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitBinaryExpr(BinaryExpr<string> expr)
	{
		return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
	}

	public string VisitCallExpr(CallExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitGetExpr(GetExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitGroupingExpr(GroupingExpr<string> expr)
	{
		return Parenthesize("group", expr.Expression);
	}

	public string VisitLiteralExpr(LiteralExpr<string> expr)
	{
		return expr.Value.ToString() ?? "nil";
	}

	public string VisitLogicalExpr(LogicalExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitSetExpr(SetExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitSuperExpr(SuperExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitThisExpr(ThisExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	public string VisitUnaryExpr(UnaryExpr<string> expr)
	{
		return Parenthesize(expr.Operator.Lexeme, expr.Right);
	}

	public string VisitVariableExpr(VariableExpr<string> expr)
	{
		throw new NotImplementedException();
	}

	private string Parenthesize(string name, params Expr<string>[] exprs)
	{
		var builder = new StringBuilder();

		builder.Append("(").Append(name);
		foreach (var expr in exprs)
		{
			builder.Append(" ");
			builder.Append(expr.Accept(this));
		}
		builder.Append(")");

		return builder.ToString();
	}
}
