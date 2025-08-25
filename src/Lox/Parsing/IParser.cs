using Lox.Statements;

namespace Lox.Parsing;

public interface IParser
{
	IList<Stmt>? Parse(IEnumerable<Token> tokens);
}
