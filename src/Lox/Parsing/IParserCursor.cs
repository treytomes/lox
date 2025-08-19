namespace Lox.Parsing;

public interface IParserCursor
{
	bool IsAtEnd { get; }
	Token Advance();
	bool Check(TokenType type);
	bool Match(params TokenType[] types);
	Token Peek();
	Token Previous();
	void ResetCursor(IEnumerable<Token> tokens);
}
