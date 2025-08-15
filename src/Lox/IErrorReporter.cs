namespace Lox;

public interface IErrorReporter
{
	bool HadError { get; }

	void ResetErrorFlag();
	void Error(int line, string message);
	void Report(int line, string where, string message);
}
