namespace Lox;

public interface IScannerFactory
{
	IScanner GetScanner(string source);
}
