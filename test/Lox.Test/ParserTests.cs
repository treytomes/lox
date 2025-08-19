using Lox.Expressions;
using Lox.Parsing;
using Lox.Scanning;

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
		var parserCursor = new ParserCursor();
		var parser = new Parser(parserCursor, errorReporter);
		var actualExpression = parser.Parse(tokens);

		Assert.NotNull(actualExpression);
		Assert.Equal(expectedExpression, actualExpression);
	}
}
