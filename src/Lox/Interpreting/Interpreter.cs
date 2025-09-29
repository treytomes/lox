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
				var flow = Execute(statement);
				if (flow is ControlFlow.Break breakFlow)
				{
					throw new RuntimeException(breakFlow.Keyword, "Break outside of loop.");
				}
				else if (flow is ControlFlow.Continue contFlow)
				{
					throw new RuntimeException(contFlow.Keyword, "Continue outside of loop.");
				}
			}
		}
		else
		{
			LastResult = null;
		}
	}

	private ControlFlow Execute(Stmt stmt)
	{
		return stmt switch
		{
			BlockStmt s => VisitBlockStmt(s),
			BreakStmt s => VisitBreakStmt(s),
			ContinueStmt s => VisitContinueStmt(s),
			ExpressionStmt s => VisitExpressionStmt(s),
			IfStmt s => VisitIfStmt(s),
			PrintStmt s => VisitPrintStmt(s),
			VarStmt s => VisitVarStmt(s),
			WhileStmt s => VisitWhileStmt(s),
			_ => throw new NotImplementedException($"Statement type {stmt.GetType()} is not implemented.")
		};
	}

	private ControlFlow ExecuteBlock(IList<Stmt> statements, Environment environment)
	{
		try
		{
			_environments.Push(environment);

			foreach (var statement in statements)
			{
				var flow = Execute(statement);
				switch (flow)
				{
					case ControlFlow.Break:
					case ControlFlow.Continue:
						// Console.WriteLine($"flow: {flow}");
						return flow;
					case ControlFlow.Next:
					default:
						continue;
				}
			}
		}
		finally
		{
			_environments.Pop();
		}

		return ControlFlow.NextFlow;
	}

	#region Statement Visitors

	public ControlFlow VisitBlockStmt(BlockStmt stmt)
	{
		var flow = ExecuteBlock(stmt.Statements, new Environment(CurrentEnvironment));
		LastResult = null;
		return flow;
	}

	public ControlFlow VisitBreakStmt(BreakStmt stmt)
	{
		LastResult = null;
		return new ControlFlow.Break(stmt.Keyword);
	}

	public ControlFlow VisitClassStmt(ClassStmt stmt)
	{
		throw new NotImplementedException();
	}

	public ControlFlow VisitContinueStmt(ContinueStmt stmt)
	{
		LastResult = null;
		return new ControlFlow.Continue(stmt.Keyword);
	}

	public ControlFlow VisitExpressionStmt(ExpressionStmt stmt)
	{
		LastResult = Evaluate(stmt.Expression);
		return ControlFlow.NextFlow;
	}

	public ControlFlow VisitFunctionStmt(FunctionStmt stmt)
	{
		throw new NotImplementedException();
	}

	public ControlFlow VisitIfStmt(IfStmt stmt)
	{
		var flow = ControlFlow.NextFlow;
		if (Evaluate(stmt.Condition).IsTruthy())
		{
			flow = Execute(stmt.ThenBranch);
		}
		else if (stmt.ElseBranch != null)
		{
			flow = Execute(stmt.ElseBranch);
		}
		LastResult = null;
		return flow;
	}

	public ControlFlow VisitPrintStmt(PrintStmt stmt)
	{
		var value = Evaluate(stmt.Expression);
		_console.WriteLine(value.Stringify(true));
		LastResult = null;
		return ControlFlow.NextFlow;
	}

	public ControlFlow VisitReturnStmt(ReturnStmt stmt)
	{
		throw new NotImplementedException();
	}

	public ControlFlow VisitVarStmt(VarStmt stmt)
	{
		object? value = null;
		if (stmt.Initializer != null)
		{
			value = Evaluate(stmt.Initializer);
		}

		CurrentEnvironment.Define(stmt.Name.Lexeme, value);
		LastResult = null;
		return ControlFlow.NextFlow;
	}

	public ControlFlow VisitWhileStmt(WhileStmt stmt)
	{
		while (Evaluate(stmt.Condition).IsTruthy())
		{
			var flow = Execute(stmt.Body);
			if (flow is ControlFlow.Break)
			{
				break;
			}
			else if (flow is ControlFlow.Continue)
			{
				// The actual "continue" effect is handled in the Block processor.
				continue;
			}
		}
		LastResult = null;
		return ControlFlow.NextFlow;
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
		var callee = Evaluate(expr.Callee);

		var arguments = new List<object?>();
		foreach (var argument in expr.Arguments)
		{
			arguments.Add(Evaluate(argument));
		}

		if (callee is ILoxCallable function)
		{
			return function.Call(this, arguments);
		}
		else
		{
			throw new RuntimeException(expr.Paren, "Expected a function.");
		}
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
