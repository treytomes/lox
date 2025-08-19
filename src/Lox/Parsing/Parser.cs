using Lox.Exceptions;
using Lox.Expressions;
using Lox.Reporting;

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

	public Expr? Parse(IEnumerable<Token> tokens)
	{
		try
		{
			_cursor.ResetCursor(tokens);
			return Expression();
		}
		catch (ParseException)
		{
			return null;
		}
	}

	private Expr Expression()
	{
		return Equality();
	}

	private Expr Equality()
	{
		var expr = Equality();

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

		if (_cursor.Match(TokenType.LEFT_PAREN))
		{
			var expr = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
			return new GroupingExpr(expr);
		}

		throw Error(_cursor.Peek(), "Expect expression.");
	}

	private Token Consume(TokenType type, string message)
	{
		if (_cursor.Check(type)) return _cursor.Advance();

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
