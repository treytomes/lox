using Lox.Expressions;
using Lox.Parsing;
using Lox.Scanning;
using Lox.Statements;

namespace Lox.Test;

public class ParserTests
{
	[Fact]
	public void CanParseTokensIntoExpression()
	{
		var expectedStatement = new ExpressionStmt(
			new BinaryExpr(
				new UnaryExpr(
					new Token(TokenType.MINUS, "-", null, 0),
					new LiteralExpr(123)),
				new Token(TokenType.STAR, "*", null, 0),
				new GroupingExpr(
					new LiteralExpr(45.67))
			)
		);

		var sourceText = "-123 * (45.67);";
		var errorReporter = new TestErrorReporter();
		var scanner = new Scanner(new ScannerCursor(), errorReporter);
		var tokens = scanner.ScanTokens(sourceText);
		var parserCursor = new ParserCursor();
		var parser = new Parser(parserCursor, errorReporter);
		var actualStatement = parser.Parse(tokens);

		Assert.NotNull(actualStatement);
		Assert.Single(actualStatement);
		Assert.Equal(expectedStatement.ToString(), actualStatement.Single().ToString());
	}
}
