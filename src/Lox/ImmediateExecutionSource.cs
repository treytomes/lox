namespace Lox;

public class ImmediateExecutionSource : IExecutionSource
{
	private readonly string _sourceText;

	public ImmediateExecutionSource(string source)
	{
		_sourceText = source;
	}

	public string GetSource()
	{
		return _sourceText;
	}

	public async Task<string> GetSourceAsync()
	{
		await Task.Yield();
		return GetSource();
	}
}
