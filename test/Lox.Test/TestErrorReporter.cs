using Lox.Exceptions;
using Lox.Reporting;

namespace Lox.Test;

public class TestErrorReporter : IErrorReporter
{
	#region Fields

	private List<LoxError> _errors = new();
	private List<RuntimeException> _runtimeErrors = new();

	#endregion

	#region Properties

	public bool HadError => _errors.Any();
	public bool HadRuntimeError => _runtimeErrors.Any();
	public IReadOnlyList<LoxError> Errors => _errors.AsReadOnly();
	public IReadOnlyList<RuntimeException> RuntimeErrors => _runtimeErrors.AsReadOnly();

	#endregion

	#region Methods

	public void Report(int line, string where, string message)
	{
		_errors.Add(new LoxError(line, where, message));
	}

	public void RuntimeError(RuntimeException error)
	{
		_runtimeErrors.Add(error);
	}

	public void ResetErrorFlags()
	{
		_errors.Clear();
		_runtimeErrors.Clear();
	}

	#endregion
}
