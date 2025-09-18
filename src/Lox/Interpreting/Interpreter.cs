using Lox.Exceptions;
using Lox.Reporting;

namespace Lox.Interpreting;

public class Interpreter : IInterpreter
{
	#region Constants

	private const string LAST_RESULT_NAME = "_";

	#endregion

	#region Fields

	private readonly Stack<IEnvironment> _environments = new();
	private readonly IOutputWriter _console;
	private readonly IErrorReporter _errorReporter;

	#endregion

	#region Constructors

	public Interpreter(IOutputWriter console, IErrorReporter errorReporter)
	{
		_environments.Push(new Environment());
		_console = console ?? throw new ArgumentNullException(nameof(console));
		_errorReporter = errorReporter ?? throw new ArgumentNullException(nameof(errorReporter));
	}

	#endregion

	#region Properties

	public IEnvironment CurrentEnvironment => _environments.Peek();
	public object? LastResult { get; private set; } = null;

	#endregion

	#region Methods

	public object? Evaluate(Expr expr)
	{
		return expr switch
		{
			AssignExpr e => VisitAssignExpr(e),
			BinaryExpr e => VisitBinaryExpr(e),
			CallExpr e => VisitCallExpr(e),
			GetExpr e => VisitGetExpr(e),
			GroupingExpr e => VisitGroupingExpr(e),
			IfExpr e => VisitIfExpr(e),
			ListExpr e => VisitListExpr(e),
			LiteralExpr e => VisitLiteralExpr(e),
			LogicalExpr e => VisitLogicalExpr(e),
			SetExpr e => VisitSetExpr(e),
			SuperExpr e => VisitSuperExpr(e),
			ThisExpr e => VisitThisExpr(e),
			UnaryExpr e => VisitUnaryExpr(e),
			VariableExpr e => VisitVariableExpr(e),
			_ => throw new NotImplementedException($"Expression type {expr.GetType()} is not implemented.")
		};
	}

	public void Interpret(IList<Stmt> statements)
	{
		if (statements.Any())
		{
			foreach (var statement in statements)
			{
				Execute(statement);
			}
		}
		else
		{
			LastResult = null;
		}
	}

	private void Execute(Stmt stmt)
	{
		var _ = stmt switch
		{
			BlockStmt s => VisitBlockStmt(s),
			ExpressionStmt s => VisitExpressionStmt(s),
			IfStmt s => VisitIfStmt(s),
			PrintStmt s => VisitPrintStmt(s),
			VarStmt s => VisitVarStmt(s),
			WhileStmt s => VisitWhileStmt(s),
			_ => throw new NotImplementedException($"Statement type {stmt.GetType()} is not implemented.")
		};
	}

	private void ExecuteBlock(IList<Stmt> statements, Environment environment)
	{
		try
		{
			_environments.Push(environment);

			foreach (var statement in statements)
			{
				Execute(statement);
			}
		}
		finally
		{
			_environments.Pop();
		}
	}

	#region Statement Visitors

	public Unit VisitBlockStmt(BlockStmt stmt)
	{
		ExecuteBlock(stmt.Statements, new Environment(CurrentEnvironment));
		LastResult = null;
		return default;
	}

	public Unit VisitClassStmt(ClassStmt stmt)
	{
		throw new NotImplementedException();
	}

	public Unit VisitExpressionStmt(ExpressionStmt stmt)
	{
		LastResult = Evaluate(stmt.Expression);
		return default;
	}

	public Unit VisitFunctionStmt(FunctionStmt stmt)
	{
		throw new NotImplementedException();
	}

	public Unit VisitIfStmt(IfStmt stmt)
	{
		if (Evaluate(stmt.Condition).IsTruthy())
		{
			Execute(stmt.ThenBranch);
		}
		else if (stmt.ElseBranch != null)
		{
			Execute(stmt.ElseBranch);
		}
		LastResult = null;
		return default;
	}

	public Unit VisitPrintStmt(PrintStmt stmt)
	{
		var value = Evaluate(stmt.Expression);
		_console.WriteLine(value.Stringify(true));
		LastResult = null;
		return default;
	}

	public Unit VisitReturnStmt(ReturnStmt stmt)
	{
		throw new NotImplementedException();
	}

	public Unit VisitVarStmt(VarStmt stmt)
	{
		object? value = null;
		if (stmt.Initializer != null)
		{
			value = Evaluate(stmt.Initializer);
		}

		CurrentEnvironment.Define(stmt.Name.Lexeme, value);
		LastResult = null;
		return default;
	}

	public Unit VisitWhileStmt(WhileStmt stmt)
	{
		while (Evaluate(stmt.Condition).IsTruthy())
		{
			Execute(stmt.Body);
		}
		LastResult = null;
		return default;
	}

	#endregion

	#region Expression Visitors

	public object? VisitAssignExpr(AssignExpr expr)
	{
		var value = Evaluate(expr.Value);
		CurrentEnvironment.Set(expr.Name, value);
		return value;
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

	public object? VisitIfExpr(IfExpr expr)
	{
		var condResult = Evaluate(expr.Condition);
		if (condResult.IsTruthy())
		{
			return Evaluate(expr.IfTrue);
		}
		return Evaluate(expr.IfFalse);
	}

	public object? VisitListExpr(ListExpr expr)
	{
		Evaluate(expr.Left);
		return Evaluate(expr.Right);
	}

	public object? VisitLiteralExpr(LiteralExpr expr)
	{
		return expr.Value;
	}

	public object? VisitLogicalExpr(LogicalExpr expr)
	{
		var left = Evaluate(expr.Left);

		if (expr.Operator.Type == TokenType.OR)
		{
			if (left.IsTruthy()) return left;
		}
		else
		{
			if (!left.IsTruthy()) return left;
		}

		return Evaluate(expr.Right);
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
		if (expr.Name.Lexeme == LAST_RESULT_NAME)
		{
			return LastResult;
		}
		return CurrentEnvironment.Get(expr.Name);
	}

	#endregion

	#region Helpers

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

	#endregion

	#endregion
}
