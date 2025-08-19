namespace Lox.Reporting;

public interface IErrorReporter
{
	bool HadError { get; }

	void ResetErrorFlag();
	void Error(int line, string message);
	void Report(int line, string where, string message);
	void Error(Token token, string message);
}
