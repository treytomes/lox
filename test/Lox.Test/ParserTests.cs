using Lox.Expressions;

namespace Lox.Test;

public class ParserTests
{
	public void CanParseTokensIntoExpression()
	{
		var expectedExpression = new BinaryExpr(
			new UnaryExpr(
				new Token(TokenType.MINUS, "-", null, 1),
				new LiteralExpr(123)),
			new Token(TokenType.STAR, "*", null, 1),
			new GroupingExpr(
				new LiteralExpr(45.67)));

		var sourceText = "-123 * (45.67)";
		var errorReporter = new TestErrorReporter();
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var parser = new Parser(errorReporter, tokens);
		var actualExpression = parser.Parse();

		Assert.NotNull(actualExpression);
		Assert.Equal(expectedExpression, actualExpression);
	}
}
