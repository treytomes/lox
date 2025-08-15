namespace Lox;

public interface IScanner
{
	IList<Token> ScanTokens(string source);
}
