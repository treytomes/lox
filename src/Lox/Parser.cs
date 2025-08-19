using Lox.Exceptions;
using Lox.Expressions;
using Lox.Reporting;

namespace Lox;

public interface IParser
{

}

public class Parser : IParser
{
	#region Fields

	private readonly IErrorReporter _errorReporter;
	private readonly IList<Token> _tokens;
	private int _current = 0;

	#endregion

	#region Constructors

	public Parser(IErrorReporter errorReporter, IList<Token> tokens)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (tokens == null) throw new ArgumentNullException(nameof(tokens));
		_errorReporter = errorReporter;
		_tokens = tokens;
	}

	#endregion

	#region Properties

	private bool IsAtEnd => Peek().Type == TokenType.EOF;

	#endregion

	#region Methods

	public Expr? Parse()
	{
		try
		{
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

		while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
		{
			var op = Previous();
			var right = Comparison();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Comparison()
	{
		var expr = Term();

		while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
		{
			var op = Previous();
			var right = Term();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Term()
	{
		var expr = Factor();

		while (Match(TokenType.MINUS, TokenType.PLUS))
		{
			var op = Previous();
			var right = Factor();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Factor()
	{
		var expr = Unary();

		while (Match(TokenType.SLASH, TokenType.STAR))
		{
			var op = Previous();
			var right = Unary();
			expr = new BinaryExpr(expr, op, right);
		}

		return expr;
	}

	private Expr Unary()
	{
		if (Match(TokenType.BANG, TokenType.MINUS))
		{
			var op = Previous();
			var right = Unary();
			return new UnaryExpr(op, right);
		}

		return Primary();
	}

	private Expr Primary()
	{
		if (Match(TokenType.FALSE)) return new LiteralExpr(false);
		if (Match(TokenType.TRUE)) return new LiteralExpr(true);
		if (Match(TokenType.NIL)) return new LiteralExpr(null);

		if (Match(TokenType.NUMBER, TokenType.STRING))
		{
			return new LiteralExpr(Previous().Literal);
		}

		if (Match(TokenType.LEFT_PAREN))
		{
			var expr = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
			return new GroupingExpr(expr);
		}

		throw Error(Peek(), "Expect expression.");
	}

	private bool Match(params TokenType[] types)
	{
		foreach (var type in types)
		{
			if (Check(type))
			{
				Advance();
				return true;
			}
		}

		return false;
	}

	private bool Check(TokenType type)
	{
		if (IsAtEnd) return false;
		return Peek().Type == type;
	}

	private Token Advance()
	{
		if (!IsAtEnd) _current++;
		return Previous();
	}

	private Token Peek()
	{
		return _tokens[_current];
	}

	private Token Previous()
	{
		return _tokens[_current - 1];
	}

	private Token Consume(TokenType type, string message)
	{
		if (Check(type)) return Advance();

		throw Error(Peek(), message);
	}

	private ParseException Error(Token token, string message)
	{
		_errorReporter.Error(token, message);
		return new ParseException();
	}

	private void Synchronize()
	{
		Advance();

		while (!IsAtEnd)
		{
			if (Previous().Type == TokenType.SEMICOLON) return;

			switch (Peek().Type)
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

			Advance();
		}
	}
	#endregion
}
