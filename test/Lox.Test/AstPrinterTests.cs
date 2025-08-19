using Lox.Expressions;

namespace Lox.Test;

public class AstPrinterTests
{
	[Fact]
	public void CanGenerateAst()
	{
		var expression = new BinaryExpr<string>(
				new UnaryExpr<string>(
					new Token(TokenType.MINUS, "-", null, 1),
					new LiteralExpr<string>(123)),
				new Token(TokenType.STAR, "*", null, 1),
				new GroupingExpr<string>(
					new LiteralExpr<string>(45.67)));

		var expected = "(* (- 123) (group 45.67))";
		var actual = new AstPrinter().ToString(expression);
		Assert.Equal(expected, actual);
	}
}
