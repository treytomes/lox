namespace Lox.Test.Extensions;

public class ExprExtensionsTests
{
	[Fact]
	public void CanGenerateAst()
	{
		var expression = new BinaryExpr(
				new UnaryExpr(
					new Token(TokenType.MINUS, "-", null, 1),
					new LiteralExpr(123)),
				new Token(TokenType.STAR, "*", null, 1),
				new GroupingExpr(
					new LiteralExpr(45.67)));

		var expected = "(* (- 123) (group 45.67))";
		var actual = expression.ToLispString();
		Assert.Equal(expected, actual);
	}
}
