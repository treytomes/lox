namespace Lox;

public class Token(TokenType type, string lexeme, object? literal, int line) : IEquatable<Token>
{
	#region Properties

	public TokenType Type => type;
	public string Lexeme => lexeme;
	public object? Literal => literal;
	public int Line => line;

	#endregion

	#region Methods

	public override string ToString()
	{
		return $"{Type} {Lexeme} {Literal}";
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as Token);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(type, lexeme, literal, line);
	}

	public bool Equals(Token? other)
	{
		if (other == null) return false;
		return (Line == other.Line) && (Type == other.Type) && (Lexeme == other.Lexeme);
	}

	#endregion
}
