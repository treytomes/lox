namespace Lox.Scanning;

public class ScannerCursor : IScannerCursor
{
	#region Fields

	private int _start = 0;
	private int _current = 0;
	private int _line = 0;
	private string _source = string.Empty;

	#endregion

	#region Properties

	public bool IsAtEnd => _current >= _source.Length;
	public int Line => _line;
	public string CurrentText => _source.Substring(_start, _current - _start);

	#endregion

	#region Methods

	public void ResetCursor(string source)
	{
		if (source == null) throw new ArgumentNullException(nameof(source));
		_start = 0;
		_current = 0;
		_line = 0;
		_source = source;
	}

	/// <inheritdoc/>
	public void BeginLexeme()
	{
		_start = _current;
	}

	public void NewLine()
	{
		_line++;
	}

	public char Advance()
	{
		return _source[_current++];
	}

	public bool Match(char expected)
	{
		if (IsAtEnd) return false;
		if (_source[_current] != expected) return false;

		_current++;
		return true;
	}

	public char Peek()
	{
		if (IsAtEnd) return '\0';
		return _source[_current];
	}

	public char PeekNext()
	{
		if (_current + 1 >= _source.Length) return '\0';
		return _source[_current + 1];
	}

	#endregion
}
