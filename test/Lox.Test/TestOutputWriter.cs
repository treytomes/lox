using Lox.Reporting;

namespace Lox.Test;

public class TestOutputWriter : IOutputWriter
{
	#region Fields

	private readonly List<string> _lines = new();

	#endregion

	#region Properties

	public IReadOnlyList<string> Lines => _lines.AsReadOnly();

	#endregion

	#region Methods

	public void WriteLine(string text)
	{
		_lines.Add(text);
	}

	#endregion
}
