using Lox.Exceptions;

namespace Lox;

public class Environment(IEnvironment? enclosing = null) : IEnvironment
{
	#region Fields

	private readonly Dictionary<string, object?> _values = new();
	private readonly IEnvironment? _enclosing = enclosing;

	#endregion

	#region Methods

	public bool IsDefined(Token name)
	{
		return IsDefined(name.Lexeme);
	}

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
		if (!IsDefined(name.Lexeme))
		{
			if (_enclosing?.IsDefined(name) ?? false)
			{
				_enclosing?.Set(name, value);
				return;
			}

			throw new RuntimeException(name, $"Variable {name} has not been defined.");
		}
		_values[name.Lexeme] = value;
	}

	public void Set(string name, object? value)
	{
		if (!IsDefined(name))
		{
			if (_enclosing?.IsDefined(name) ?? false)
			{
				_enclosing?.Set(name, value);
				return;
			}

			throw new ApplicationException($"Variable {name} has not been defined.");
		}

		_values[name] = value;
	}

	public object? Get(Token name)
	{
		if (!IsDefined(name.Lexeme))
		{
			if (_enclosing?.IsDefined(name) ?? false)
			{
				return _enclosing?.Get(name);
			}

			throw new RuntimeException(name, $"Variable {name} has not been defined.");
		}

		return _values[name.Lexeme];
	}

	public object? Get(string name)
	{
		if (!IsDefined(name))
		{
			if (_enclosing?.IsDefined(name) ?? false)
			{
				return _enclosing?.Get(name);
			}

			throw new ApplicationException($"Variable {name} has not been defined.");
		}

		return _values[name];
	}

	#endregion
}
