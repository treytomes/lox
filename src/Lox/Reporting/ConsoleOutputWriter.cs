namespace Lox.Reporting;

public class ConsoleOutputWriter : IOutputWriter
{
	public void WriteLine(string text)
	{
		Console.WriteLine(text);
	}
}
