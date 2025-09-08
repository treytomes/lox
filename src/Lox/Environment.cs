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
		return IsLocallyDefined(name) || (_enclosing?.IsDefined(name) ?? false);
	}

	private bool IsLocallyDefined(Token name)
	{
		return IsLocallyDefined(name.Lexeme);
	}

	private bool IsLocallyDefined(string name)
	{
		return _values.ContainsKey(name);
	}

	public void Define(Token name, object? value = null)
	{
		Define(name.Lexeme, value);
	}

	public void Define(string name, object? value = null)
	{
		if (IsLocallyDefined(name))
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
		if (IsLocallyDefined(name))
		{
			_values[name.Lexeme] = value;
		}
		else if (IsDefined(name.Lexeme))
		{
			_enclosing?.Set(name, value);
		}
		else
		{
			throw new RuntimeException(name, $"Variable {name.Lexeme} has not been defined.");
		}
	}

	public void Set(string name, object? value)
	{
		if (IsLocallyDefined(name))
		{
			_values[name] = value;
		}
		else if (IsDefined(name))
		{
			_enclosing?.Set(name, value);
		}
		else
		{
			throw new ApplicationException($"Variable {name} has not been defined.");
		}
	}

	public object? Get(Token name)
	{
		if (IsLocallyDefined(name))
		{
			return _values[name.Lexeme];
		}
		else if (IsDefined(name))
		{
			return _enclosing?.Get(name);
		}
		throw new RuntimeException(name, $"Variable {name.Lexeme} has not been defined.");
	}

	public object? Get(string name)
	{
		if (IsLocallyDefined(name))
		{
			return _values[name];
		}
		else if (IsDefined(name))
		{
			return _enclosing?.Get(name);
		}
		throw new ApplicationException($"Variable {name} has not been defined.");
	}

	#endregion
}
