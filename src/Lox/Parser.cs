using Lox.Expressions;

namespace Lox;

public interface IParser
{

}

public class Parser : IParser
{
	#region Fields

	private readonly IList<Token> _tokens;
	private int _current = 0;

	#endregion

	#region Constructors

	public Parser(IList<Token> tokens)
	{
		if (tokens == null) throw new ArgumentNullException(nameof(tokens));
		_tokens = tokens;
	}

	#endregion

	#region Properties

	private bool IsAtEnd => Peek().Type == TokenType.EOF;

	#endregion

	#region Methods

	private Expr<object?> Expression()
	{
		return Equality();
	}

	private Expr<object?> Equality()
	{
		var expr = Equality();

		while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
		{
			var op = Previous();
			var right = Comparison();
			expr = new BinaryExpr<object?>(expr, op, right);
		}

		return expr;
	}

	private Expr<object?> Comparison()
	{
		var expr = Term();

		while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
		{
			var op = Previous();
			var right = Term();
			expr = new BinaryExpr<object?>(expr, op, right);
		}

		return expr;
	}

	private Expr<object?> Term()
	{
		var expr = Factor();

		while (Match(TokenType.MINUS, TokenType.PLUS))
		{
			var op = Previous();
			var right = Factor();
			expr = new BinaryExpr<object?>(expr, op, right);
		}

		return expr;
	}

	private Expr<object?> Factor()
	{
		var expr = Unary();

		while (Match(TokenType.SLASH, TokenType.STAR))
		{
			var op = Previous();
			var right = Unary();
			expr = new BinaryExpr<object?>(expr, op, right);
		}

		return expr;
	}

	private Expr<object?> Unary()
	{
		if (Match(TokenType.BANG, TokenType.MINUS))
		{
			var op = Previous();
			var right = Unary();
			return new UnaryExpr<object?>(op, right);
		}

		return Primary();
	}

	private Expr<object?> Primary()
	{
		if (Match(TokenType.FALSE)) return new LiteralExpr<object?>(false);
		if (Match(TokenType.TRUE)) return new LiteralExpr<object?>(true);
		if (Match(TokenType.NIL)) return new LiteralExpr<object?>(null);

		if (Match(TokenType.NUMBER, TokenType.STRING))
		{
			return new LiteralExpr<object?>(Previous().Literal);
		}

		if (Match(TokenType.LEFT_PAREN))
		{
			var expr = Expression();
			Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
			return new GroupingExpr<object?>(expr);
		}
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

	#endregion
}
