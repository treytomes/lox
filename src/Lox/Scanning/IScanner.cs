namespace Lox.Scanning;

public interface IScanner
{
	IList<Token> ScanTokens(string source);
}
