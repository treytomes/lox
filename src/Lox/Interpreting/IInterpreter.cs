namespace Lox.Interpreting;

public interface IInterpreter
{
	IEnvironment CurrentEnvironment { get; }
	object? LastResult { get; }

	object? Evaluate(Expr expr);
	void Interpret(IList<Stmt> statements);
}
