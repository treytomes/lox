
using System.Collections.ObjectModel;

namespace Lox;

public class Scanner : IScanner
{
	#region Fields

	private static readonly ReadOnlyDictionary<string, TokenType> _keywords = new(new Dictionary<string, TokenType>()
	{
		{ "and", TokenType.AND },
		{ "class", TokenType.CLASS },
		{ "else", TokenType.ELSE },
		{ "false", TokenType.FALSE },
		{ "for", TokenType.FOR },
		{ "fun", TokenType.FUN },
		{ "if", TokenType.IF },
		{ "nil", TokenType.NIL },
		{ "or", TokenType.OR },
		{ "print", TokenType.PRINT },
		{ "return", TokenType.RETURN },
		{ "super", TokenType.SUPER },
		{ "this", TokenType.THIS },
		{ "true", TokenType.TRUE },
		{ "var", TokenType.VAR },
		{ "while", TokenType.WHILE },
	});

	private readonly IErrorReporter _errorReporter;
	private readonly string _source;
	private readonly IList<Token> _tokens = new List<Token>();
	private int _start = 0;
	private int _current = 0;
	private int _line = 0;

	#endregion

	#region Constructors

	public Scanner(IErrorReporter errorReporter, string source)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (source == null) throw new ArgumentNullException(nameof(source));
		_errorReporter = errorReporter;
		_source = source;
	}

	#endregion

	#region Properties

	private bool IsAtEnd => _current >= _source.Length;

	#endregion

	#region Methods

	public IList<Token> ScanTokens(string source)
	{
		while (!IsAtEnd)
		{
			// We are at the beginning of the next lexeme.
			_start = _current;
			ScanToken();
		}

		_tokens.Add(new Token(TokenType.EOF, string.Empty, null, _line));
		return _tokens;
	}

	private void ScanToken()
	{
		char c = Advance();
		switch (c)
		{
			case '(': AddToken(TokenType.LEFT_PAREN); break;
			case ')': AddToken(TokenType.RIGHT_PAREN); break;
			case '{': AddToken(TokenType.LEFT_BRACE); break;
			case '}': AddToken(TokenType.RIGHT_BRACE); break;
			case ',': AddToken(TokenType.COMMA); break;
			case '.': AddToken(TokenType.DOT); break;
			case '-': AddToken(TokenType.MINUS); break;
			case '+': AddToken(TokenType.PLUS); break;
			case ';': AddToken(TokenType.SEMICOLON); break;
			case '*': AddToken(TokenType.STAR); break;
			case '!':
				AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
				break;
			case '=':
				AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
				break;
			case '<':
				AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
				break;
			case '>':
				AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
				break;
			case '/':
				if (Match('/'))
				{
					// A line comment goes until the end of the line.
					while (Peek() != '\n' && !IsAtEnd) Advance();
				}
				else if (Match('*'))
				{
					// A block comment goes until the end of the block comment marker.
					while (Peek() != '*' && PeekNext() != '/') Advance();
				}
				else
				{
					AddToken(TokenType.SLASH);
				}
				break;

			case ' ':
			case '\r':
			case '\t':
				// Ignore whitespace.
				break;

			case '\n':
				_line++;
				break;

			case '"': String(); break;

			default:
				if (IsDigit(c))
				{
					Number();
				}
				else if (IsAlpha(c))
				{
					Identifier();
				}
				else
				{
					_errorReporter.Error(_line, "Unexpected character.");
				}
				break;
		}
	}

	private char Advance()
	{
		return _source[_current++];
	}

	private void AddToken(TokenType type)
	{
		AddToken(type, null);
	}

	private void AddToken(TokenType type, object? literal)
	{
		var text = _source.Substring(_start, _current - _start);
		_tokens.Add(new Token(type, text, literal, _line));
	}

	private bool Match(char expected)
	{
		if (IsAtEnd) return false;
		if (_source[_current] != expected) return false;

		_current++;
		return true;
	}

	private char Peek()
	{
		if (IsAtEnd) return '\0';
		return _source[_current];
	}

	private char PeekNext()
	{
		if (_current + 1 >= _source.Length) return '\0';
		return _source[_current + 1];
	}

	private void String()
	{
		while (Peek() != '"' && !IsAtEnd)
		{
			if (Peek() == '\n') _line++;
			Advance();
		}

		if (IsAtEnd)
		{
			_errorReporter.Error(_line, "Unterminated string.");
			return;
		}

		// The closing ".
		Advance();

		// Trim the surrounding quotes.
		var value = _source.Substring(_start + 1, _current - _start - 2);
		AddToken(TokenType.STRING, value);
	}

	private void Identifier()
	{
		while (IsAlphaNumeric(Peek())) Advance();

		var text = _source.Substring(_start, _current - _start);
		var type = _keywords.GetValueOrDefault(text, TokenType.IDENTIFIER);
		AddToken(type);
	}

	private bool IsAlpha(char c)
	{
		return (c >= 'a' && c <= 'z') ||
			   (c >= 'A' && c <= 'Z') ||
				c == '_';
	}

	private bool IsDigit(char c)
	{
		return c >= '0' && c <= '9';
	}

	private bool IsAlphaNumeric(char c)
	{
		return IsAlpha(c) || IsDigit(c);
	}

	private void Number()
	{
		while (IsDigit(Peek())) Advance();

		// Look for a fractional part.
		if (Peek() == '.' && IsDigit(PeekNext()))
		{
			// Consume the "."
			Advance();

			while (IsDigit(Peek())) Advance();
		}

		AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current)));
	}

	#endregion
}
