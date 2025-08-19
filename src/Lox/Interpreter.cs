using Lox.Exceptions;
using Lox.Expressions;
using Lox.Reporting;

namespace Lox;

public interface IInterpreter : IVisitor<object?>
{
	void Interpret(Expr expression);
}

public class Interpreter : IInterpreter
{
	#region Fields

	private readonly IErrorReporter _errorReporter;

	#endregion

	#region Constructors

	public Interpreter(IErrorReporter errorReporter)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		_errorReporter = errorReporter;
	}

	#endregion

	#region Methods
	public void Interpret(Expr expression)
	{
		try
		{
			var value = Evaluate(expression);
			Console.WriteLine(Stringify(value));
		}
		catch (RuntimeException ex)
		{
			_errorReporter.RuntimeError(ex);
		}
	}
	public object? VisitAssignExpr(AssignExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitBinaryExpr(BinaryExpr expr)
	{
		var left = Evaluate(expr.Left);
		var right = Evaluate(expr.Right);
		if (left is null || right is null) throw new RuntimeException(expr.Operator, "Operands must not be null.");

		switch (expr.Operator.Type)
		{
			case TokenType.BANG_EQUAL:
				return !left.IsEqual(right);

			case TokenType.EQUAL_EQUAL:
				return left.IsEqual(right);

			case TokenType.GREATER:
				CheckNumberOperands(expr.Operator, left, right);
				return (double)left > (double)right;

			case TokenType.GREATER_EQUAL:
				CheckNumberOperands(expr.Operator, left, right);
				return (double)left >= (double)right;

			case TokenType.LESS:
				CheckNumberOperands(expr.Operator, left, right);
				return (double)left < (double)right;

			case TokenType.LESS_EQUAL:
				CheckNumberOperands(expr.Operator, left, right);
				return (double)left <= (double)right;

			case TokenType.MINUS:
				CheckNumberOperands(expr.Operator, left, right);
				return (double)left - (double)right;

			case TokenType.PLUS:
				if (left is double && right is double)
				{
					return (double)left + (double)right;
				}

				if (left is string && right is string)
				{
					return (string)left + (string)right;
				}

				throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.");

			case TokenType.SLASH:
				return (double)left / (double)right;

			case TokenType.STAR:
				return (double)left * (double)right;
		}

		// Unreachable.
		return null;
	}

	public object? VisitCallExpr(CallExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitGetExpr(GetExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitGroupingExpr(GroupingExpr expr)
	{
		return Evaluate(expr.Expression);
	}

	public object? VisitLiteralExpr(LiteralExpr expr)
	{
		return expr.Value;
	}

	public object? VisitLogicalExpr(LogicalExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitSetExpr(SetExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitSuperExpr(SuperExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitThisExpr(ThisExpr expr)
	{
		throw new NotImplementedException();
	}

	public object? VisitUnaryExpr(UnaryExpr expr)
	{
		var right = Evaluate(expr.Right);
		if (right == null) throw new RuntimeException(expr.Operator, "Value cannot be null.");

		switch (expr.Operator.Type)
		{
			case TokenType.BANG:
				return !right.IsTruthy();
			case TokenType.MINUS:
				if (right is not double) throw new RuntimeException(expr.Operator, "Expected a number.");
				return -(double)right;
		}

		// Unreachable.
		return null;
	}

	public object? VisitVariableExpr(VariableExpr expr)
	{
		throw new NotImplementedException();
	}

	private object? Evaluate(Expr expr)
	{
		return expr.Accept(this);
	}

	private void CheckNumberOperand(Token op, object operand)
	{
		if (operand is double) return;
		throw new RuntimeException(op, "Operand must be a number.");
	}

	private void CheckNumberOperands(Token op, params object[] operands)
	{
		if (operands.All(x => x is double)) return;
		throw new RuntimeException(op, "Operands must be numbers.");
	}

	private string Stringify(object? obj)
	{
		if (obj == null) return "nil";

		if (obj is double)
		{
			var text = obj.ToString();
			if (text == null) throw new NullReferenceException("This shouldn't have happened.");
			if (text.EndsWith(".0"))
			{
				text = text.Substring(0, text.Length - 2);
			}
			return text;
		}

		return obj.ToString() ?? string.Empty;
	}

	#endregion
}
