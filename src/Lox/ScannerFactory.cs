namespace Lox;

public class ScannerFactory : IScannerFactory
{
	private readonly IErrorReporter _errorReporter;

	public ScannerFactory(IErrorReporter errorReporter)
	{
		if (errorReporter == null) throw new ArgumentNullException(nameof(errorReporter));
		_errorReporter = errorReporter;
	}

	public IScanner GetScanner(string source)
	{
		return new Scanner(_errorReporter, source);
	}
}
