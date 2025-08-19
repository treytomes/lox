namespace Lox.Parsing;

public class ParserCursor : IParserCursor
{
	#region Fields

	private IReadOnlyList<Token> _tokens = new List<Token>().AsReadOnly();
	private int _current = 0;

	#endregion

	#region Properties

	public bool IsAtEnd => Peek().Type == TokenType.EOF;

	#endregion

	#region Methods

	public Token Advance()
	{
		if (!IsAtEnd) _current++;
		return Previous();
	}

	public bool Check(TokenType type)
	{
		if (IsAtEnd) return false;
		return Peek().Type == type;
	}

	public bool Match(params TokenType[] types)
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

	public void ResetCursor(IEnumerable<Token> tokens)
	{
		if (tokens == null) throw new ArgumentNullException(nameof(tokens));
		_tokens = tokens.ToList().AsReadOnly();
		_current = 0;
	}

	public Token Peek()
	{
		return _tokens[_current];
	}

	public Token Previous()
	{
		return _tokens[_current - 1];
	}

	#endregion
}
