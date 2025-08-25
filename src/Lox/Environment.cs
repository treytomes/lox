using System.Reflection.Metadata.Ecma335;
using Lox.Exceptions;

namespace Lox;

public class Environment : IEnvironment
{
	#region Fields

	private readonly Dictionary<string, object?> _values = new();

	#endregion

	#region Methods

	public bool IsDefined(string name)
	{
		return _values.ContainsKey(name);
	}

	public void Define(Token name, object? value = null)
	{
		Define(name.Lexeme, value);
	}

	public void Define(string name, object? value = null)
	{
		// AssertNotDeclared(name); // Intentional choice to support REPL.
		if (IsDefined(name))
		{
			Set(name, value);
		}
		else
		{
			_values.Add(name, value);
		}
	}

	public void Set(Token name, object? value)
	{
		AssertDefined(name);
		_values[name.Lexeme] = value;
	}

	public void Set(string name, object? value)
	{
		AssertDefined(name);
		_values[name] = value;
	}

	public object? Get(Token name)
	{
		AssertDefined(name);
		return _values[name.Lexeme];
	}

	public object? Get(string name)
	{
		AssertDefined(name);
		return _values[name];
	}

	private void AssertDefined(Token name)
	{
		if (!IsDefined(name.Lexeme))
		{
			throw new RuntimeException(name, $"Variable {name} has not been defined.");
		}
	}

	private void AssertDefined(string name)
	{
		if (!IsDefined(name))
		{
			throw new ApplicationException($"Variable {name} has not been defined.");
		}
	}

	#endregion
}
