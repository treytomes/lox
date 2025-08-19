using Lox.Expressions;

namespace Lox.Parsing;

public interface IParser
{
	Expr? Parse(IEnumerable<Token> tokens);
}
