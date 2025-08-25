namespace Lox;

public interface IEnvironment
{
	bool IsDefined(string name);
	void Define(Token name, object? value = null);
	void Define(string name, object? value = null);
	void Set(Token name, object? value);
	void Set(string name, object? value);
	object? Get(Token name);
	object? Get(string name);
}
