using Lox.Reporting;

namespace Lox.Test;

public class TestErrorReporter : IErrorReporter
{
	#region Fields

	private List<LoxError> _errors = new();

	#endregion

	#region Properties

	public bool HadError => _errors.Any();
	public IReadOnlyList<LoxError> Errors => _errors.AsReadOnly();

	#endregion

	#region Methods

	public void Report(int line, string where, string message)
	{
		_errors.Add(new LoxError(line, where, message));
	}

	public void ResetErrorFlag()
	{
		_errors.Clear();
	}

	#endregion
}
