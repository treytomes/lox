namespace Lox;

public class FileExecutionSource : IExecutionSource
{
	public FileExecutionSource(string path)
	{
		if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

		Path = path;
	}

	public string Path { get; }

	public string GetSource()
	{
		return File.ReadAllText(Path);
	}

	public async Task<string> GetSourceAsync()
	{
		if (!File.Exists(Path)) throw new FileNotFoundException($"Path does not exist: {Path}");
		return await File.ReadAllTextAsync(Path);
	}
}
