using Lox.Exceptions;
using Lox.Expressions;
using Lox.Reporting;
using Lox.Statements;

namespace Lox.Visitors;

public class Interpreter : IInterpreter
{
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

	#endregion

	#region Methods

	public object? Evaluate(Expr expr)
	{
		return expr.Accept(this);
	}

	public void Interpret(IList<Stmt> statements)
	{
		foreach (var statement in statements)
		{
			Execute(statement);
		}
	}

	private void Execute(Stmt stmt)
	{
		stmt.Accept(this);
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

	public void VisitBlockStmt(BlockStmt stmt)
	{
		ExecuteBlock(stmt.Statements, new Environment(CurrentEnvironment));
	}

	public void VisitClassStmt(ClassStmt stmt)
	{
		throw new NotImplementedException();
	}

	public void VisitExpressionStmt(ExpressionStmt stmt)
	{
		Evaluate(stmt.Expression);
	}

	public void VisitFunctionStmt(FunctionStmt stmt)
	{
		throw new NotImplementedException();
	}

	public void VisitIfStmt(IfStmt stmt)
	{
		if (Evaluate(stmt.Condition).IsTruthy())
		{
			Execute(stmt.ThenBranch);
		}
		else if (stmt.ElseBranch != null)
		{
			Execute(stmt.ElseBranch);
		}
	}

	public void VisitPrintStmt(PrintStmt stmt)
	{
		var value = Evaluate(stmt.Expression);
		_console.WriteLine(value.Stringify(true));
	}

	public void VisitReturnStmt(ReturnStmt stmt)
	{
		throw new NotImplementedException();
	}

	public void VisitVarStmt(VarStmt stmt)
	{
		object? value = null;
		if (stmt.Initializer != null)
		{
			value = Evaluate(stmt.Initializer);
		}

		CurrentEnvironment.Define(stmt.Name.Lexeme, value);
	}

	public void VisitWhileStmt(WhileStmt stmt)
	{
		while (Evaluate(stmt.Condition).IsTruthy())
		{
			Execute(stmt.Body);
		}
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
