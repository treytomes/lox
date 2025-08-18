namespace Lox;

public interface IScannerCursor
{
	bool IsAtEnd { get; }
	int Line { get; }
	string CurrentText { get; }

	void ResetCursor(string source);

	/// <summary>
	/// We are at the beginning of the next lexeme.
	/// </summary>
	void BeginLexeme();

	void NewLine();
	char Advance();
	bool Match(char expected);
	char Peek();
	char PeekNext();
}
