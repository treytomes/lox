
using System.Collections.ObjectModel;
using Lox.Reporting;

namespace Lox.Scanning;

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
	private readonly IList<Token> _tokens = new List<Token>();
	private readonly IScannerCursor _cursor;

	#endregion

	#region Constructors

	public Scanner(IScannerCursor cursor, IErrorReporter errorReporter)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		if (cursor == null) throw new ArgumentNullException(nameof(cursor));
		_errorReporter = errorReporter;
		_cursor = cursor;
	}

	#endregion

	#region Methods

	public IList<Token> ScanTokens(string source)
	{
		_cursor.ResetCursor(source);
		_tokens.Clear();

		while (!_cursor.IsAtEnd)
		{
			_cursor.BeginLexeme();
			ScanToken();
		}

		_tokens.Add(new Token(TokenType.EOF, string.Empty, null, _cursor.Line));
		return _tokens;
	}

	private void ScanToken()
	{
		char c = _cursor.Advance();
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
			case '?': AddToken(TokenType.QUESTION_MARK); break;
			case ':': AddToken(TokenType.COLON); break;
			case '!':
				AddToken(_cursor.Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
				break;
			case '=':
				AddToken(_cursor.Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
				break;
			case '<':
				AddToken(_cursor.Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
				break;
			case '>':
				AddToken(_cursor.Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
				break;
			case '/':
				if (_cursor.Match('/'))
				{
					// A line comment goes until the end of the line.
					while (_cursor.Peek() != '\n' && !_cursor.IsAtEnd) _cursor.Advance();
				}
				else if (_cursor.Match('*'))
				{
					// A block comment goes until the end of the block comment marker.
					while (_cursor.Peek() != '*' && _cursor.PeekNext() != '/') _cursor.Advance();

					var foundEndMarker = _cursor.Match('*') && _cursor.Match('/');
					if (!foundEndMarker)
					{
						_errorReporter.Error(_cursor.Line, "Expected close of block comment.");
					}
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
				_cursor.NewLine();
				break;

			case '"': String(); break;

			default:
				if (c.IsDigit())
				{
					Number();
				}
				else if (c.IsAlpha())
				{
					Identifier();
				}
				else
				{
					_errorReporter.Error(_cursor.Line, "Unexpected character.");
				}
				break;
		}
	}

	private void AddToken(TokenType type)
	{
		AddToken(type, null);
	}

	private void AddToken(TokenType type, object? literal)
	{
		var text = _cursor.CurrentText;
		_tokens.Add(new Token(type, text, literal, _cursor.Line));
	}

	private void String()
	{
		while (_cursor.Peek() != '"' && !_cursor.IsAtEnd)
		{
			if (_cursor.Peek() == '\n') _cursor.NewLine();
			_cursor.Advance();
		}

		if (_cursor.IsAtEnd)
		{
			_errorReporter.Error(_cursor.Line, "Unterminated string.");
			return;
		}

		// The closing ".
		_cursor.Advance();

		// Trim the surrounding quotes.
		var text = _cursor.CurrentText;
		var value = text.Substring(1, text.Length - 2);
		AddToken(TokenType.STRING, value);
	}

	private void Identifier()
	{
		while (_cursor.Peek().IsAlphaNumeric()) _cursor.Advance();

		var text = _cursor.CurrentText;
		var type = _keywords.GetValueOrDefault(text, TokenType.IDENTIFIER);
		AddToken(type);
	}

	private void Number()
	{
		while (_cursor.Peek().IsDigit()) _cursor.Advance();

		// Look for a fractional part.
		if (_cursor.Peek() == '.' && _cursor.PeekNext().IsDigit())
		{
			// Consume the "."
			_cursor.Advance();

			while (_cursor.Peek().IsDigit()) _cursor.Advance();
		}

		AddToken(TokenType.NUMBER, double.Parse(_cursor.CurrentText));
	}

	#endregion
}
