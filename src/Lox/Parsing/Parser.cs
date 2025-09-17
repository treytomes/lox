using Lox.Exceptions;
using Lox.Expressions;
using Lox.Reporting;
using Lox.Statements;

namespace Lox.Parsing;

public class Parser : IParser
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IParserCursor _cursor;

	#endregion

	#region Constructors

	public Parser(IParserCursor cursor, IErrorReporter errorReporter)
	{
		if (cursor == null) throw new ArgumentNullException(nameof(cursor));
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		_cursor = cursor;
		_errorReporter = errorReporter;
	}

	#endregion

	#region Methods

	/// <returns>Null if there was a parsing exception.  Not sure if that's the best idea though.</returns>
	public IList<Stmt>? Parse(IEnumerable<Token> tokens)
	{
		try
		{
			List<Stmt> statements = new();
			_cursor.ResetCursor(tokens);
			while (!_cursor.IsAtEnd)
			{
				var stmt = Declaration();
				if (stmt != null)
				{
					statements.Add(stmt);
				}
			}
			return statements;
		}
		catch (ParseException)
		{
			return null;
		}
	}

	private Stmt? Declaration()
	{
		try
		{
			if (_cursor.Match(TokenType.VAR)) return VarDeclaration();

			return Statement();
		}
		catch (ParseException)
		{
			Synchronize();
			return null;
		}
	}

	private Stmt VarDeclaration()
	{
		var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

		Expr? initializer = null;
		if (_cursor.Match(TokenType.EQUAL))
		{
			initializer = Expression();
		}

		Consume([TokenType.SEMICOLON, TokenType.EOF], "Expect ';' after variable declaration.");
		return new VarStmt(name, initializer);
	}

	private Stmt Statement()
	{
		if (_cursor.Match(TokenType.FOR)) return ForStatement();
		if (_cursor.Match(TokenType.IF)) return IfStatement();
		if (_cursor.Match(TokenType.PRINT)) return PrintStatement();
		if (_cursor.Match(TokenType.WHILE)) return WhileStatement();
		if (_cursor.Match(TokenType.LEFT_BRACE)) return new BlockStmt(Block());

		return ExpressionStatement();
	}

	private Stmt ForStatement()
	{
		Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

		Stmt? initializer;
		if (_cursor.Match(TokenType.SEMICOLON))
		{
			initializer = null;
		}
		else if (_cursor.Match(TokenType.VAR))
		{
			initializer = VarDeclaration();
		}
		else
		{
			initializer = ExpressionStatement();
		}


		Expr? condition = null;
		if (!_cursor.Check(TokenType.SEMICOLON))
		{
			condition = Expression();
		}
		Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

		Expr? increment = null;
		if (!_cursor.Check(TokenType.RIGHT_PAREN))
		{
			increment = Expression();
		}
		Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");
		var body = Statement();

		if (increment != null)
		{
			body = new BlockStmt([
				body,
				new ExpressionStmt(increment),
			]);
		}

		if (condition == null) condition = new LiteralExpr(true);
		body = new WhileStmt(condition, body);

		if (initializer != null)
		{
			body = new BlockStmt([initializer, body]);
		}
		return body;
	}

	private Stmt IfStatement()
	{
		Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
		var condition = Expression();
		Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

		var thenBranch = Statement();
		var elseBranch = (Stmt?)null;
		if (_cursor.Match(TokenType.ELSE))
		{
			elseBranch = Statement();
		}

		return new IfStmt(condition, thenBranch, elseBranch);
	}

	private Stmt PrintStatement()
	{
		var value = Expression();
		Consume([TokenType.SEMICOLON, TokenType.EOF], "Expect ';' after value.");
		return new PrintStmt(value);
	}

	private Stmt WhileStatement()
	{
		Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
		var condition = Expression();
		Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
		var body = Statement();
		return new WhileStmt(condition, body);
	}

	private List<Stmt> Block()
	{
		var statements = new List<Stmt>();

		while (!_cursor.Check(TokenType.RIGHT_BRACE) && !_cursor.IsAtEnd)
		{
			var stmt = Declaration();
			if (stmt != null)
			{
				statements.Add(stmt);
			}
		}

		Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
		return statements;
	}

	private Stmt ExpressionStatement()
	{
		var expr = Expression();
		Consume([TokenType.SEMICOLON, TokenType.EOF], "Expect ';' after expression.");
		return new ExpressionStmt(expr);
	}

	private Expr Expression()
	{
		return ExpressionList();
	}

	private Expr ExpressionList()
	{
		var expr = Assignment();

		while (_cursor.Match(TokenType.COMMA))
		{
			var right = Assignment();
			expr = new ListExpr(expr, right);
		}

		return expr;
	}

	private Expr Assignment()
	{
		var expr = Or();

		if (_cursor.Match(TokenType.EQUAL))
		{
			var equals = _cursor.Previous();
			var value = Assignment();

			if (expr is VariableExpr)
			{
				var name = ((VariableExpr)expr).Name;
				return new AssignExpr(name, value);
			}

			_errorReporter.Error(equals, "Invalid assignment target.");
		}

		return expr;
	}

	private Expr Or()
	{
		var expr = And();

		while (_cursor.Match(TokenType.OR))
		{
			var op = _cursor.Previous();
			var right = And();
			expr = new LogicalExpr(expr, op, right);
		}

		return expr;
	}

	private Expr And()
	{
		var expr = Equality();

		while (_cursor.Match(TokenType.AND))
		{
			var op = _cursor.Previous();
			var right = Equality();
			expr = new LogicalExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Equality()
	{
		var expr = Comparison();

		while (_cursor.Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
		{
			var op = _cursor.Previous();
			var right = Comparison();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Comparison()
	{
		var expr = Term();

		while (_cursor.Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
		{
			var op = _cursor.Previous();
			var right = Term();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Term()
	{
		var expr = Factor();

		while (_cursor.Match(TokenType.MINUS, TokenType.PLUS))
		{
			var op = _cursor.Previous();
			var right = Factor();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Factor()
	{
		var expr = Unary();

		while (_cursor.Match(TokenType.SLASH, TokenType.STAR))
		{
			var op = _cursor.Previous();
			var right = Unary();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Unary()
	{
		if (_cursor.Match(TokenType.BANG, TokenType.MINUS))
		{
			var op = _cursor.Previous();
			var right = Unary();
			return new UnaryExpr(op, right);
		}

		return Primary();
	}

	private Expr Primary()
	{
		if (_cursor.Match(TokenType.FALSE)) return new LiteralExpr(false);
		if (_cursor.Match(TokenType.TRUE)) return new LiteralExpr(true);
		if (_cursor.Match(TokenType.NIL)) return new LiteralExpr(null);

		if (_cursor.Match(TokenType.NUMBER, TokenType.STRING))
		{
			return new LiteralExpr(_cursor.Previous().Literal);
		}

		if (_cursor.Match(TokenType.IDENTIFIER))
		{
			return new VariableExpr(_cursor.Previous());
		}

		if (_cursor.Match(TokenType.LEFT_PAREN))
		{
			var expr = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
			return new GroupingExpr(expr);
		}

		throw Error(_cursor.Peek(), "Expect expression.");
	}

	private Token Consume(TokenType type, string message) => Consume([type], message);

	private Token Consume(TokenType[] types, string message)
	{
		if (_cursor.Check(types)) return _cursor.Advance();

		throw Error(_cursor.Peek(), message);
	}

	private ParseException Error(Token token, string message)
	{
		_errorReporter.Error(token, message);
		return new ParseException();
	}

	private void Synchronize()
	{
		_cursor.Advance();

		while (!_cursor.IsAtEnd)
		{
			if (_cursor.Previous().Type == TokenType.SEMICOLON) return;

			switch (_cursor.Peek().Type)
			{
				case TokenType.CLASS:
				case TokenType.FUN:
				case TokenType.VAR:
				case TokenType.FOR:
				case TokenType.IF:
				case TokenType.WHILE:
				case TokenType.PRINT:
				case TokenType.RETURN:
					return;
			}

			_cursor.Advance();
		}
	}

	#endregion
}
