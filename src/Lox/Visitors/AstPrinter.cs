using System.Text;
using Lox.Expressions;

namespace Lox.Visitors;

public class AstPrinter : IExprVisitor<string>
{
	public string ToString(Expr expr)
	{
		return expr.Accept(this);
	}

	public string VisitAssignExpr(AssignExpr expr)
	{
		throw new NotImplementedException();
	}

	public string VisitBinaryExpr(BinaryExpr expr)
	{
		return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
	}

	public string VisitCallExpr(CallExpr expr)
	{
		throw new NotImplementedException();
	}

	public string VisitGetExpr(GetExpr expr)
	{
		throw new NotImplementedException();
	}

	public string VisitGroupingExpr(GroupingExpr expr)
	{
		return Parenthesize("group", expr.Expression);
	}

	public string VisitListExpr(ListExpr expr)
	{
		return Parenthesize(string.Empty, expr.Left, expr.Right);
	}

	public string VisitLiteralExpr(LiteralExpr expr)
	{
		return expr.Value?.ToString() ?? "nil";
	}

	public string VisitLogicalExpr(LogicalExpr expr)
	{
		return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
	}

	public string VisitSetExpr(SetExpr expr)
	{
		throw new NotImplementedException();
	}

	public string VisitSuperExpr(SuperExpr expr)
	{
		throw new NotImplementedException();
	}

	public string VisitThisExpr(ThisExpr expr)
	{
		throw new NotImplementedException();
	}

	public string VisitUnaryExpr(UnaryExpr expr)
	{
		return Parenthesize(expr.Operator.Lexeme, expr.Right);
	}

	public string VisitVariableExpr(VariableExpr expr)
	{
		throw new NotImplementedException();
	}

	private string Parenthesize(string name, params Expr[] exprs)
	{
		var builder = new StringBuilder();

		builder.Append("(");
		if (!string.IsNullOrWhiteSpace(name))
		{
			builder.Append(name).Append(" ");
		}

		builder.Append(string.Join(" ", exprs.Select(x => x.Accept(this))));
		builder.Append(")");

		return builder.ToString();
	}
}
